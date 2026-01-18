using System; // Import base system types
using System.Collections.Generic; // Import generic collection types
using System.Collections.ObjectModel; // Import observable collections for WPF
using System.IO; // Import file and directory IO utilities
using System.Linq; // Import LINQ helpers used in parsing
using cashregister.Model; // Import model types like CartItem
using Microsoft.Data.Sqlite; // Import SQLite ADO.NET provider
using System.Net; // HtmlEncode for safe html output
using System.Text; // StringBuilder for html rendering
using cashregister.Common; // ReceiptRecord definition moved to Common

namespace cashregister.Services // Namespace for services
{
    // Encapsulates receipt storage, numbering, saving, loading, and one-time migration from legacy txt files
    public class ReceiptService // Service responsible for persistence of receipts
    {
        public ReceiptService() // Constructor
        {
            EnsureReceiptsFolder(); // Ensure storage folder and DB are ready on creation
        }

        public void EnsureReceiptsFolder() // Ensure the Receipts folder exists and initialize DB
        {
            var dir = GetReceiptsFolder(); // Resolve the Receipts folder path
            if (!Directory.Exists(dir)) // If the folder does not exist
            {
                Directory.CreateDirectory(dir); // Create the folder
            }
            EnsureDatabase(); // Ensure database file and schema are created
            EnsureHtmlFolder(); // Ensure html mirror folder exists
            ExportAllReceiptsToHtml(); // Keep html mirror in sync with DB on startup
        }

        public string GetReceiptsFolder() // Compute Receipts folder path
        {
            // Place SavedInfo at the project root alongside Model and ViewModel
            // During debugging, BaseDirectory points to bin/Debug/... so go up three levels
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..")); // Resolve project root
            return Path.Combine(projectRoot, "SavedInfo"); // Combine to SavedInfo subfolder
        }

        private string GetHtmlReceiptsFolder() => Path.Combine(GetReceiptsFolder(), "html"); // Subfolder for html receipts
        private string GetDatabasePath() => Path.Combine(GetReceiptsFolder(), "receipts.db"); // Path to SQLite file
        private string GetConnectionString() => new SqliteConnectionStringBuilder { DataSource = GetDatabasePath() }.ToString(); // Build SQLite connection string

        private void EnsureDatabase() // Create database and tables if missing and migrate columns
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    PRAGMA foreign_keys = ON;
                    CREATE TABLE IF NOT EXISTS Receipts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Number INTEGER NOT NULL UNIQUE,
                        DateUtc TEXT NOT NULL,
                        PaymentMethod TEXT NOT NULL,
                        Subtotal REAL NOT NULL,
                        Gst REAL NOT NULL,
                        Qst REAL NOT NULL,
                        TotalDiscountPercent REAL NOT NULL,
                        Total REAL NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS ReceiptItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ReceiptNumber INTEGER NOT NULL,
                        Name TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Quantity INTEGER NOT NULL,
                        DiscountPercent REAL NOT NULL,
                        Subtotal REAL NOT NULL,
                        FOREIGN KEY (ReceiptNumber) REFERENCES Receipts(Number) ON DELETE CASCADE
                    );
                    CREATE TABLE IF NOT EXISTS Inventory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Price REAL NOT NULL,
                        Category TEXT NOT NULL
                    );
                ";
                cmd.ExecuteNonQuery();
            }

            // Lightweight migration to add new flags/columns as needed
            var receiptCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var info = conn.CreateCommand())
            {
                info.CommandText = "PRAGMA table_info(Receipts)";
                using var r = info.ExecuteReader();
                while (r.Read()) receiptCols.Add(r.GetString(1));
            }
            if (!receiptCols.Contains("Refunded"))
            {
                using var alter = conn.CreateCommand(); alter.CommandText = "ALTER TABLE Receipts ADD COLUMN Refunded INTEGER NOT NULL DEFAULT 0"; alter.ExecuteNonQuery();
            }
            if (!receiptCols.Contains("IsRefund"))
            {
                using var alter = conn.CreateCommand(); alter.CommandText = "ALTER TABLE Receipts ADD COLUMN IsRefund INTEGER NOT NULL DEFAULT 0"; alter.ExecuteNonQuery();
            }
            if (!receiptCols.Contains("RefundOfNumber"))
            {
                using var alter = conn.CreateCommand(); alter.CommandText = "ALTER TABLE Receipts ADD COLUMN RefundOfNumber INTEGER"; alter.ExecuteNonQuery();
            }
            if (!receiptCols.Contains("RefundStatus"))
            {
                using var alter = conn.CreateCommand(); alter.CommandText = "ALTER TABLE Receipts ADD COLUMN RefundStatus INTEGER NOT NULL DEFAULT 0"; alter.ExecuteNonQuery();
            }

            var itemCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var info = conn.CreateCommand())
            {
                info.CommandText = "PRAGMA table_info(ReceiptItems)";
                using var r = info.ExecuteReader();
                while (r.Read()) itemCols.Add(r.GetString(1));
            }
            if (!itemCols.Contains("Refunded"))
            {
                using var alter = conn.CreateCommand(); alter.CommandText = "ALTER TABLE ReceiptItems ADD COLUMN Refunded INTEGER NOT NULL DEFAULT 0"; alter.ExecuteNonQuery();
            }
            if (!itemCols.Contains("RefundStatus"))
            {
                using var alter = conn.CreateCommand(); alter.CommandText = "ALTER TABLE ReceiptItems ADD COLUMN RefundStatus INTEGER NOT NULL DEFAULT 0"; alter.ExecuteNonQuery();
            }
        }

        private void EnsureHtmlFolder()
        {
            var htmlDir = GetHtmlReceiptsFolder();
            if (!Directory.Exists(htmlDir))
            {
                Directory.CreateDirectory(htmlDir);
            }
        }

        private static decimal ParseDecimalFromCurrency(string input) // Parse currency-like text to decimal
        {
            var digits = new string(input.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray()); // Keep digits, dot, comma
            if (!decimal.TryParse(digits, out var value)) value = 0m; // Try parse; fallback to 0
            return value; // Return parsed decimal
        }

        public int GetNextReceiptNumber() // Compute next receipt number from DB
        {
            EnsureReceiptsFolder(); // Ensure storage is ready
            using var conn = new SqliteConnection(GetConnectionString()); // Create connection
            conn.Open(); // Open connection
            using var cmd = conn.CreateCommand(); // Create command
            cmd.CommandText = "SELECT IFNULL(MAX(Number), 0) FROM Receipts"; // Query max receipt number or 0
            var result = cmd.ExecuteScalar(); // Execute scalar query
            var max = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0; // Convert to int with null check
            return max + 1; // Next number is max + 1
        }

        public void SaveReceipt(int number, string paymentMethod, IEnumerable<CartItem> cart, decimal subtotal, decimal gst, decimal qst, decimal totalDiscountPercent, decimal total) // Persist a receipt and its items
        {
            EnsureReceiptsFolder(); // Ensure storage is ready
            using var conn = new SqliteConnection(GetConnectionString()); // Create connection
            conn.Open(); // Open connection
            using var tx = conn.BeginTransaction(); // Begin transaction for atomic writes

            var nowUtc = DateTime.UtcNow; // capture once for DB and HTML

            using (var cmd = conn.CreateCommand()) // Insert receipt header
            {
                cmd.Transaction = tx; // Bind to transaction
                // Parameterized insert into Receipts
                cmd.CommandText = @"
                    INSERT INTO Receipts(Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total)
                    VALUES ($num, $date, $method, $sub, $gst, $qst, $tdp, $tot);
                ";
                cmd.Parameters.AddWithValue("$num", number); // Set receipt number
                cmd.Parameters.AddWithValue("$date", nowUtc.ToString("o")); // Set ISO-8601 UTC date
                cmd.Parameters.AddWithValue("$method", paymentMethod); // Set payment method
                cmd.Parameters.AddWithValue("$sub", subtotal); // Set subtotal
                cmd.Parameters.AddWithValue("$gst", gst); // Set GST
                cmd.Parameters.AddWithValue("$qst", qst); // Set QST
                cmd.Parameters.AddWithValue("$tdp", totalDiscountPercent); // Set discount percent (0..1)
                cmd.Parameters.AddWithValue("$tot", total); // Set total
                cmd.ExecuteNonQuery(); // Execute insert
            }

            using (var cmd = conn.CreateCommand()) // Insert receipt items
            {
                cmd.Transaction = tx; // Bind to transaction
                // Parameterized insert into ReceiptItems
                cmd.CommandText = @"
                    INSERT INTO ReceiptItems(ReceiptNumber, Name, Price, Quantity, DiscountPercent, Subtotal)
                    VALUES ($num, $name, $price, $qty, $disc, $sub);
                ";
                var pNum = cmd.CreateParameter(); pNum.ParameterName = "$num"; cmd.Parameters.Add(pNum); // Prepare parameter: receipt number
                var pName = cmd.CreateParameter(); pName.ParameterName = "$name"; cmd.Parameters.Add(pName); // Prepare parameter: item name
                var pPrice = cmd.CreateParameter(); pPrice.ParameterName = "$price"; cmd.Parameters.Add(pPrice); // Prepare parameter: item price
                var pQty = cmd.CreateParameter(); pQty.ParameterName = "$qty"; cmd.Parameters.Add(pQty); // Prepare parameter: item quantity
                var pDisc = cmd.CreateParameter(); pDisc.ParameterName = "$disc"; cmd.Parameters.Add(pDisc); // Prepare parameter: item discount
                var pSub = cmd.CreateParameter(); pSub.ParameterName = "$sub"; cmd.Parameters.Add(pSub); // Prepare parameter: item subtotal

                foreach (var ci in cart) // Loop over cart items
                {
                    pNum.Value = number; // Assign receipt number
                    pName.Value = ci.Name; // Assign item name
                    pPrice.Value = ci.Price; // Assign item price
                    pQty.Value = ci.Quantity; // Assign item quantity
                    pDisc.Value = ci.DiscountPercent; // Assign item discount percent
                    pSub.Value = ci.Subtotal; // Assign item subtotal
                    cmd.ExecuteNonQuery(); // Execute insert for this item
                }
            }

            tx.Commit(); // Commit transaction to persist changes

            // Write/update HTML mirror
            WriteHtmlReceipt(number, nowUtc, paymentMethod, cart, subtotal, gst, qst, totalDiscountPercent, total);
        }

        public bool TryLoadReceipt(int number, ObservableCollection<CartItem> cart, out decimal totalDiscountPercent) // Load receipt by number
        {
            totalDiscountPercent = 0m; // Initialize out parameter
            EnsureReceiptsFolder(); // Ensure storage is ready
            using var conn = new SqliteConnection(GetConnectionString()); // Create connection
            conn.Open(); // Open connection

            using (var cmd = conn.CreateCommand()) // Query total discount percent from header
            {
                cmd.CommandText = "SELECT TotalDiscountPercent FROM Receipts WHERE Number = $num"; // Select discount percent
                cmd.Parameters.AddWithValue("$num", number); // Bind receipt number
                var v = cmd.ExecuteScalar(); // Execute scalar
                if (v == null || v == DBNull.Value) // If no receipt found
                {
                    return false; // Indicate failure to load
                }
                totalDiscountPercent = Convert.ToDecimal(v); // Convert discount percent to decimal
            }

            var items = new List<CartItem>(); // Temporary list for items
            using (var cmd = conn.CreateCommand()) // Query items for receipt
            {
                // Select item fields ordered by Id
                cmd.CommandText = @"
                    SELECT Name, Price, Quantity, DiscountPercent
                    FROM ReceiptItems
                    WHERE ReceiptNumber = $num
                    ORDER BY Id ASC;
                ";
                cmd.Parameters.AddWithValue("$num", number); // Bind receipt number
                using var reader = cmd.ExecuteReader(); // Execute reader for multiple rows
                while (reader.Read()) // Iterate returned rows
                {
                    var name = reader.GetString(0); // Read item name
                    var price = Convert.ToDecimal(reader.GetValue(1)); // Read item price (REAL -> decimal)
                    var qty = reader.GetInt32(2); // Read item quantity
                    var disc = Convert.ToDecimal(reader.GetValue(3)); // Read item discount percent
                    items.Add(new CartItem { Name = name, Price = price, Quantity = qty, DiscountPercent = disc }); // Add to list
                }
            }

            cart.Clear(); // Clear current cart
            foreach (var it in items) cart.Add(it); // Populate cart with loaded items
            return true; // Indicate successful load
        }

        public IEnumerable<ReceiptRecord> GetReceiptHistory(int max = 100) // Get latest receipts with items for history view
        {
            var records = new List<ReceiptRecord>(); // Collect results
            using var conn = new SqliteConnection(GetConnectionString()); // Open DB
            conn.Open();

            var receiptCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var info = conn.CreateCommand())
            {
                info.CommandText = "PRAGMA table_info(Receipts)";
                using var r = info.ExecuteReader();
                while (r.Read()) receiptCols.Add(r.GetString(1));
            }
            bool hasRefunded = receiptCols.Contains("Refunded");
            bool hasIsRefund = receiptCols.Contains("IsRefund");
            bool hasRefundOf = receiptCols.Contains("RefundOfNumber");
            bool hasRefundStatus = receiptCols.Contains("RefundStatus");

            var selectHead = new StringBuilder();
            selectHead.Append("SELECT Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total");
            selectHead.Append(hasRefunded ? ", Refunded" : ", 0 as Refunded");
            selectHead.Append(hasIsRefund ? ", IsRefund" : ", 0 as IsRefund");
            selectHead.Append(hasRefundOf ? ", RefundOfNumber" : ", NULL as RefundOfNumber");
            selectHead.Append(hasRefundStatus ? ", RefundStatus" : ", 0 as RefundStatus");
            selectHead.Append(" FROM Receipts ORDER BY DateUtc DESC, Number DESC LIMIT $max;");

            // Query latest headers, most recent first
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = selectHead.ToString();
                cmd.Parameters.AddWithValue("$max", max);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var rec = new ReceiptRecord
                    {
                        Number = reader.GetInt32(0),
                        DateUtc = DateTime.Parse(reader.GetString(1)),
                        PaymentMethod = reader.GetString(2),
                        Subtotal = Convert.ToDecimal(reader.GetValue(3)),
                        Gst = Convert.ToDecimal(reader.GetValue(4)),
                        Qst = Convert.ToDecimal(reader.GetValue(5)),
                        TotalDiscountPercent = Convert.ToDecimal(reader.GetValue(6)),
                        Total = Convert.ToDecimal(reader.GetValue(7)),
                        Refunded = Convert.ToInt32(reader.GetValue(8)) != 0,
                        IsRefund = Convert.ToInt32(reader.GetValue(9)) != 0,
                        RefundOfNumber = reader.IsDBNull(10) ? null : reader.GetInt32(10)
                    };
                    // Map tri-state on receipts: 0 normal, 1 refunded, 2 refunddetails
                    var status = hasRefundStatus ? Convert.ToInt32(reader.GetValue(11)) : (rec.IsRefund ? 2 : (rec.Refunded ? 1 : 0));
                    rec.RefundStatus = status;
                    records.Add(rec);
                }
            }

            // Detect item columns
            var itemCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var info = conn.CreateCommand())
            {
                info.CommandText = "PRAGMA table_info(ReceiptItems)";
                using var r = info.ExecuteReader();
                while (r.Read()) itemCols.Add(r.GetString(1));
            }
            bool hasItemRefunded = itemCols.Contains("Refunded");
            bool hasItemRefundStatus = itemCols.Contains("RefundStatus");

            foreach (var rec in records)
            {
                using var icmd = conn.CreateCommand();
                var selectItems = new StringBuilder();
                selectItems.Append("SELECT Name, Price, Quantity, DiscountPercent");
                if (hasItemRefundStatus) selectItems.Append(", RefundStatus"); else if (hasItemRefunded) selectItems.Append(", Refunded"); else selectItems.Append(", 0 as RefundStatus");
                selectItems.Append(" FROM ReceiptItems WHERE ReceiptNumber = $num ORDER BY Id ASC;");
                icmd.CommandText = selectItems.ToString();
                icmd.Parameters.AddWithValue("$num", rec.Number);
                using var r = icmd.ExecuteReader();
                while (r.Read())
                {
                    var rs = (hasItemRefundStatus ? Convert.ToInt32(r.GetValue(4)) : (hasItemRefunded ? (Convert.ToInt32(r.GetValue(4)) != 0 ? 1 : 0) : 0));
                    rec.Items.Add(new CartItem
                    {
                        Name = r.GetString(0),
                        Price = Convert.ToDecimal(r.GetValue(1)),
                        Quantity = r.GetInt32(2),
                        DiscountPercent = Convert.ToDecimal(r.GetValue(3)),
                        RefundStatus = rs
                    });
                }
            }

            return records; // Return history
        }

        public void DeleteReceiptNoShift(int number) // Refund entire receipt (delete without renumbering)
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var tx = conn.BeginTransaction();

            // Mark header as refunded (soft), and create a refund receipt with negative total equal to original
            ReceiptRecord? rec = LoadReceiptRecord(conn, number);
            if (rec == null) { tx.Rollback(); return; }

            using (var mark = conn.CreateCommand())
            {
                mark.Transaction = tx;
                mark.CommandText = "UPDATE Receipts SET Refunded = 1, RefundStatus = 1 WHERE Number = $n";
                mark.Parameters.AddWithValue("$n", number);
                mark.ExecuteNonQuery();
            }

            int refundNumber = GetNextReceiptNumber();
            using (var createRefund = conn.CreateCommand())
            {
                createRefund.Transaction = tx;
                createRefund.CommandText = @"
                    INSERT INTO Receipts(Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total, Refunded, IsRefund, RefundOfNumber, RefundStatus)
                    VALUES ($num, $date, $method, $sub, $gst, $qst, $tdp, $tot, 0, 1, $orig, 2);
                ";
                createRefund.Parameters.AddWithValue("$num", refundNumber);
                createRefund.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
                createRefund.Parameters.AddWithValue("$method", "Refund");
                createRefund.Parameters.AddWithValue("$sub", -rec.Subtotal);
                createRefund.Parameters.AddWithValue("$gst", -rec.Gst);
                createRefund.Parameters.AddWithValue("$qst", -rec.Qst);
                createRefund.Parameters.AddWithValue("$tdp", 0m);
                createRefund.Parameters.AddWithValue("$tot", -rec.Total);
                createRefund.Parameters.AddWithValue("$orig", rec.Number);
                createRefund.ExecuteNonQuery();
            }

            tx.Commit();

            // Write HTML for the created refund receipt
            var refundRec = LoadReceiptRecord(refundNumber);
            if (refundRec != null) WriteHtmlReceipt(refundRec);
        }

        public void RefundSingleItem(int receiptNumber, string itemName, decimal price, int quantity, decimal discountPercent)
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var tx = conn.BeginTransaction();

            long? targetRowId = null;
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    SELECT rowid FROM ReceiptItems
                    WHERE ReceiptNumber = $num AND Name = $name AND Price = $price AND Quantity = $qty AND DiscountPercent = $disc AND COALESCE(RefundStatus,0) = 0
                    LIMIT 1;
                ";
                cmd.Parameters.AddWithValue("$num", receiptNumber);
                cmd.Parameters.AddWithValue("$name", itemName);
                cmd.Parameters.AddWithValue("$price", price);
                cmd.Parameters.AddWithValue("$qty", quantity);
                cmd.Parameters.AddWithValue("$disc", discountPercent);
                var v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value) { tx.Rollback(); return; }
                targetRowId = (long)v;
            }

            using (var upd = conn.CreateCommand())
            {
                upd.Transaction = tx;
                upd.CommandText = "UPDATE ReceiptItems SET Refunded = 1, RefundStatus = 1 WHERE rowid = $rid";
                upd.Parameters.AddWithValue("$rid", targetRowId);
                upd.ExecuteNonQuery();
            }

            // Create refund receipt with a single negative line mirroring the refunded item
            int refundNumber = GetNextReceiptNumber();
            using (var icmd = conn.CreateCommand())
            {
                icmd.Transaction = tx;
                icmd.CommandText = @"
                    INSERT INTO Receipts(Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total, Refunded, IsRefund, RefundOfNumber, RefundStatus)
                    VALUES ($num, $date, $method, $sub, $gst, $qst, $tdp, $tot, 0, 1, $orig, 2);
                ";
                var lineSubtotal = Math.Round(price * quantity * (1 - discountPercent), 2); // round to 2dp to match UI
                var refundSubtotal = -lineSubtotal;
                var refundGst = -Math.Round(lineSubtotal * 0.05m, 2);
                var refundQst = -Math.Round(lineSubtotal * 0.09975m, 2);
                var refundTotal = refundSubtotal + refundGst + refundQst;
                icmd.Parameters.AddWithValue("$num", refundNumber);
                icmd.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
                icmd.Parameters.AddWithValue("$method", "Refund");
                icmd.Parameters.AddWithValue("$sub", refundSubtotal);
                icmd.Parameters.AddWithValue("$gst", refundGst);
                icmd.Parameters.AddWithValue("$qst", refundQst);
                icmd.Parameters.AddWithValue("$tdp", 0m);
                icmd.Parameters.AddWithValue("$tot", refundTotal);
                icmd.Parameters.AddWithValue("$orig", receiptNumber);
                icmd.ExecuteNonQuery();
            }

            using (var addItem = conn.CreateCommand())
            {
                addItem.Transaction = tx;
                addItem.CommandText = @"
                    INSERT INTO ReceiptItems(ReceiptNumber, Name, Price, Quantity, DiscountPercent, Subtotal, Refunded, RefundStatus)
                    VALUES ($num, $name, $price, $qty, $disc, $sub, 0, 2);
                ";
                addItem.Parameters.AddWithValue("$num", refundNumber);
                addItem.Parameters.AddWithValue("$name", itemName);
                addItem.Parameters.AddWithValue("$price", -price); // negative line
                addItem.Parameters.AddWithValue("$qty", quantity);
                addItem.Parameters.AddWithValue("$disc", discountPercent);
                addItem.Parameters.AddWithValue("$sub", -Math.Round(price * quantity * (1 - discountPercent), 2));
                addItem.ExecuteNonQuery();
            }

            tx.Commit();

            // Write HTML for the created refund detail receipt
            var refundRec = LoadReceiptRecord(refundNumber);
            if (refundRec != null) WriteHtmlReceipt(refundRec);
        }

        // --- HTML mirroring helpers ---
        private void ExportAllReceiptsToHtml()
        {
            EnsureHtmlFolder();
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();

            var numbers = new List<int>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Number FROM Receipts ORDER BY Number ASC";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    numbers.Add(reader.GetInt32(0));
                }
            }

            foreach (var num in numbers)
            {
                var rec = LoadReceiptRecord(conn, num);
                if (rec != null)
                {
                    WriteHtmlReceipt(rec);
                }
            }
        }

        private ReceiptRecord? LoadReceiptRecord(int number)
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            return LoadReceiptRecord(conn, number);
        }

        private ReceiptRecord? LoadReceiptRecord(SqliteConnection conn, int number)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total
                FROM Receipts WHERE Number = $n;
            ";
            cmd.Parameters.AddWithValue("$n", number);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var rec = new ReceiptRecord
            {
                Number = reader.GetInt32(0),
                DateUtc = DateTime.Parse(reader.GetString(1)),
                PaymentMethod = reader.GetString(2),
                Subtotal = Convert.ToDecimal(reader.GetValue(3)),
                Gst = Convert.ToDecimal(reader.GetValue(4)),
                Qst = Convert.ToDecimal(reader.GetValue(5)),
                TotalDiscountPercent = Convert.ToDecimal(reader.GetValue(6)),
                Total = Convert.ToDecimal(reader.GetValue(7))
            };

            using var icmd = conn.CreateCommand();
            icmd.CommandText = @"
                SELECT Name, Price, Quantity, DiscountPercent
                FROM ReceiptItems WHERE ReceiptNumber = $n ORDER BY Id ASC;
            ";
            icmd.Parameters.AddWithValue("$n", number);
            using var r = icmd.ExecuteReader();
            while (r.Read())
            {
                rec.Items.Add(new CartItem
                {
                    Name = r.GetString(0),
                    Price = Convert.ToDecimal(r.GetValue(1)),
                    Quantity = r.GetInt32(2),
                    DiscountPercent = Convert.ToDecimal(r.GetValue(3))
                });
            }

            return rec;
        }

        private void WriteHtmlReceipt(ReceiptRecord rec)
        {
            WriteHtmlReceipt(rec.Number, rec.DateUtc, rec.PaymentMethod, rec.Items, rec.Subtotal, rec.Gst, rec.Qst, rec.TotalDiscountPercent, rec.Total);
        }

        private void WriteHtmlReceipt(int number, DateTime dateUtc, string paymentMethod, IEnumerable<CartItem> items, decimal subtotal, decimal gst, decimal qst, decimal totalDiscountPercent, decimal total)
        {
            EnsureHtmlFolder();
            var path = Path.Combine(GetHtmlReceiptsFolder(), $"receipt-{number}.html");

            var list = items.ToList();
            string titleSuffix = string.Empty;
            if (string.Equals(paymentMethod, "Refund", StringComparison.OrdinalIgnoreCase) && list.Count == 1)
            {
                var it = list[0];
                var amountAbs = Math.Abs(it.Subtotal);
                titleSuffix = $" (REFUND DETAILS of {WebUtility.HtmlEncode(it.Name)} -{amountAbs:0.00}$)";
            }

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset=\"utf-8\"><title>Receipt ");
            sb.Append(WebUtility.HtmlEncode(number.ToString()));
            sb.Append("</title><style>body{font-family:Segoe UI,Arial,sans-serif;margin:20px}table{border-collapse:collapse;width:100%}th,td{border:1px solid #ddd;padding:8px;text-align:left}th{background:#f2f2f2}tfoot td{font-weight:bold}</style></head><body>");
            sb.Append("<h2>Receipt #"); sb.Append(WebUtility.HtmlEncode(number.ToString())); sb.Append(WebUtility.HtmlEncode(titleSuffix)); sb.Append("</h2>");
            sb.Append("<div><strong>Date (UTC):</strong> "); sb.Append(WebUtility.HtmlEncode(dateUtc.ToString("u"))); sb.Append("</div>");
            sb.Append("<div><strong>Payment:</strong> "); sb.Append(WebUtility.HtmlEncode(paymentMethod)); sb.Append("</div>");
            sb.Append("<hr/>");
            sb.Append("<table><thead><tr><th>Item</th><th>Qty</th><th>Price</th><th>Discount %</th><th>Subtotal</th></tr></thead><tbody>");
            foreach (var it in list)
            {
                sb.Append("<tr><td>"); sb.Append(WebUtility.HtmlEncode(it.Name)); sb.Append("</td>");
                sb.Append("<td>"); sb.Append(it.Quantity.ToString()); sb.Append("</td>");
                sb.Append("<td>"); sb.Append(it.Price.ToString("0.00")); sb.Append("</td>");
                sb.Append("<td>"); sb.Append((it.DiscountPercent * 100m).ToString("0.##")); sb.Append("%</td>");
                sb.Append("<td>"); sb.Append(it.Subtotal.ToString("0.00")); sb.Append("</td></tr>");
            }
            sb.Append("</tbody><tfoot>");
            sb.Append("<tr><td colspan=\"4\">Subtotal</td><td>"); sb.Append(subtotal.ToString("0.00")); sb.Append("</td></tr>");
            sb.Append("<tr><td colspan=\"4\">GST (5%)</td><td>"); sb.Append(gst.ToString("0.00")); sb.Append("</td></tr>");
            sb.Append("<tr><td colspan=\"4\">QST (9.975%)</td><td>"); sb.Append(qst.ToString("0.00")); sb.Append("</td></tr>");
            if (totalDiscountPercent > 0)
            {
                sb.Append("<tr><td colspan=\"4\">Order Discount</td><td>"); sb.Append((totalDiscountPercent * 100m).ToString("0.##")); sb.Append("%</td></tr>");
            }
            sb.Append("<tr><td colspan=\"4\">Total</td><td>"); sb.Append(total.ToString("0.00")); sb.Append("</td></tr>");
            sb.Append("</tfoot></table>");
            sb.Append("</body></html>");

            File.WriteAllText(path, sb.ToString());
        }

        public string GetHtmlReceiptPath(int number) => Path.Combine(GetHtmlReceiptsFolder(), $"receipt-{number}.html");

        public string EnsureHtmlReceipt(int number)
        {
            EnsureHtmlFolder();
            var path = GetHtmlReceiptPath(number);
            if (!File.Exists(path))
            {
                var rec = LoadReceiptRecord(number);
                if (rec != null)
                {
                    WriteHtmlReceipt(rec);
                }
            }
            return path;
        }

        public void InitializeAll() // Reset refund details and delete refund receipts
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx; cmd.CommandText = "DELETE FROM Receipts WHERE COALESCE(IsRefund,0)=1"; cmd.ExecuteNonQuery();
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx; cmd.CommandText = "UPDATE Receipts SET Refunded = 0, RefundStatus = 0 WHERE COALESCE(Refunded,0)<>0 OR COALESCE(RefundStatus,0)<>0"; cmd.ExecuteNonQuery();
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx; cmd.CommandText = "UPDATE ReceiptItems SET Refunded = 0, RefundStatus = 0"; cmd.ExecuteNonQuery();
            }

            tx.Commit();

            // Regenerate HTML mirrors
            ExportAllReceiptsToHtml();
        }

        public void FactoryReset()
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var tx = conn.BeginTransaction();
            using (var cmd = conn.CreateCommand()) { cmd.Transaction = tx; cmd.CommandText = "DELETE FROM Receipts"; cmd.ExecuteNonQuery(); }
            using (var cmd = conn.CreateCommand()) { cmd.Transaction = tx; cmd.CommandText = "DELETE FROM ReceiptItems"; cmd.ExecuteNonQuery(); }
            using (var cmd = conn.CreateCommand()) { cmd.Transaction = tx; cmd.CommandText = "DELETE FROM Inventory"; cmd.ExecuteNonQuery(); }
            tx.Commit();
            EnsureHtmlFolder();
            // Delete all html files
            var htmlDir = GetHtmlReceiptsFolder();
            if (Directory.Exists(htmlDir))
            {
                foreach (var file in Directory.GetFiles(htmlDir)) File.Delete(file);
            }
        }

        public List<Item> LoadInventory()
        {
            var list = new List<Item>();
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            // Check if table exists (it should via EnsureDatabase)
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Price, Category FROM Inventory ORDER BY Category, Name";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Item
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Price = Convert.ToDecimal(r.GetValue(2)),
                    Category = r.GetString(3)
                });
            }
            return list;
        }

        public void SaveInventoryItem(Item item)
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var cmd = conn.CreateCommand();
            if (item.Id > 0)
            {
                cmd.CommandText = "UPDATE Inventory SET Name=$n, Price=$p, Category=$c WHERE Id=$i";
                cmd.Parameters.AddWithValue("$i", item.Id);
            }
            else
            {
                // check name uniqueness or replace
                cmd.CommandText = "INSERT OR REPLACE INTO Inventory(Name, Price, Category) VALUES($n, $p, $c)";
            }
            cmd.Parameters.AddWithValue("$n", item.Name);
            cmd.Parameters.AddWithValue("$p", item.Price);
            cmd.Parameters.AddWithValue("$c", item.Category);
            cmd.ExecuteNonQuery();

            if (item.Id == 0)
            {
                // update ID
                cmd.CommandText = "SELECT last_insert_rowid()";
                cmd.Parameters.Clear();
                item.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void DeleteInventoryItem(Item item)
        {
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Inventory WHERE Id=$i";
            cmd.Parameters.AddWithValue("$i", item.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
