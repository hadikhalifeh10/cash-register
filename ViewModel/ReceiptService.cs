using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using cashregister.Model;
using Microsoft.Data.Sqlite;

namespace cashregister.ViewModel
{
    // Encapsulates receipt storage, numbering, saving, loading, and one-time migration from legacy txt files
    public class ReceiptService
    {
        public ReceiptService()
        {
            EnsureReceiptsFolder();
        }

        public void EnsureReceiptsFolder()
        {
            var dir = GetReceiptsFolder();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            EnsureDatabase();
        }

        public string GetReceiptsFolder()
        {
            // Place Receipts at the project root alongside Model and ViewModel
            // During debugging, BaseDirectory points to bin/Debug/... so go up three levels
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            return Path.Combine(projectRoot, "Receipts");
        }

        private string GetDatabasePath() => Path.Combine(GetReceiptsFolder(), "receipts.db");
        private string GetConnectionString() => new SqliteConnectionStringBuilder { DataSource = GetDatabasePath() }.ToString();

        private void EnsureDatabase()
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
                ";
                cmd.ExecuteNonQuery();
            }

            // One-time migration of legacy txt receipts
            MigrateLegacyTextReceipts(conn);
        }

        private void MigrateLegacyTextReceipts(SqliteConnection conn)
        {
            var dir = GetReceiptsFolder();
            if (!Directory.Exists(dir)) return;
            var txtFiles = Directory.GetFiles(dir, "*.txt");
            if (txtFiles.Length == 0) return;

            foreach (var file in txtFiles)
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (!int.TryParse(name, out var number) || number <= 0)
                    {
                        // Not a numeric receipt file, skip but remove to enforce discontinuation
                        continue;
                    }

                    // Skip if this receipt already exists
                    bool exists;
                    using (var check = conn.CreateCommand())
                    {
                        check.CommandText = "SELECT 1 FROM Receipts WHERE Number = $num";
                        check.Parameters.AddWithValue("$num", number);
                        exists = check.ExecuteScalar() != null;
                    }

                    if (!exists)
                    {
                        // Parse legacy file
                        var lines = File.ReadAllLines(file);
                        var items = new List<CartItem>();
                        string payment = "Unknown";
                        DateTime dateLocal = DateTime.Now;
                        decimal subtotal = 0m, gst = 0m, qst = 0m, total = 0m, totalDiscountPercent = 0m;

                        foreach (var line in lines)
                        {
                            if (line.StartsWith("Date:"))
                            {
                                var text = line.Substring("Date:".Length).Trim();
                                if (DateTime.TryParse(text, out var d)) dateLocal = d;
                            }
                            else if (line.StartsWith("Payment:"))
                            {
                                payment = line.Substring("Payment:".Length).Trim();
                            }
                            else if (line.StartsWith("Subtotal:"))
                            {
                                subtotal = ParseDecimalFromCurrency(line.Substring("Subtotal:".Length));
                            }
                            else if (line.StartsWith("GST:"))
                            {
                                gst = ParseDecimalFromCurrency(line.Substring("GST:".Length));
                            }
                            else if (line.StartsWith("QST:"))
                            {
                                qst = ParseDecimalFromCurrency(line.Substring("QST:".Length));
                            }
                            else if (line.StartsWith("Total:"))
                            {
                                total = ParseDecimalFromCurrency(line.Substring("Total:".Length));
                            }
                            else if (line.StartsWith("Total Discount:"))
                            {
                                var percentText = line.Substring("Total Discount:".Length).Trim();
                                percentText = percentText.Trim('%');
                                if (decimal.TryParse(percentText, out var p)) totalDiscountPercent = p / 100m;
                            }
                            else if (line.StartsWith("- "))
                            {
                                // Item line parsing reused from legacy loader
                                try
                                {
                                    var s = line.Substring(2);
                                    var atIdx = s.IndexOf(" @ ");
                                    var left = s.Substring(0, atIdx);
                                    var nameEndIdx = left.LastIndexOf(" x");
                                    var itemName = left.Substring(0, nameEndIdx);
                                    var qty = int.Parse(left.Substring(nameEndIdx + 2));

                                    var right = s.Substring(atIdx + 3);
                                    var eqIdx = right.LastIndexOf(" = ");
                                    var pricePart = right.Substring(0, eqIdx).Trim();
                                    var discountPercent = 0m;
                                    var discountIdx = pricePart.IndexOf("(-");
                                    if (discountIdx >= 0)
                                    {
                                        var priceOnly = pricePart.Substring(0, discountIdx).Trim();
                                        var discountPart = pricePart.Substring(discountIdx + 2).Trim(); // like "10%)"
                                        discountPart = discountPart.TrimEnd(')');
                                        discountPart = discountPart.TrimEnd('%');
                                        decimal.TryParse(discountPart, out discountPercent);
                                        discountPercent /= 100m;
                                        pricePart = priceOnly;
                                    }
                                    var price = ParseDecimalFromCurrency(pricePart);

                                    items.Add(new CartItem { Name = itemName, Price = price, Quantity = qty, DiscountPercent = discountPercent });
                                }
                                catch
                                {
                                    // ignore parse errors
                                }
                            }
                        }

                        using var tx = conn.BeginTransaction();
                        using (var icmd = conn.CreateCommand())
                        {
                            icmd.Transaction = tx;
                            icmd.CommandText = @"
                                INSERT INTO Receipts(Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total)
                                VALUES ($num, $date, $method, $sub, $gst, $qst, $tdp, $tot);
                            ";
                            icmd.Parameters.AddWithValue("$num", number);
                            icmd.Parameters.AddWithValue("$date", dateLocal.ToUniversalTime().ToString("o"));
                            icmd.Parameters.AddWithValue("$method", payment);
                            icmd.Parameters.AddWithValue("$sub", subtotal);
                            icmd.Parameters.AddWithValue("$gst", gst);
                            icmd.Parameters.AddWithValue("$qst", qst);
                            icmd.Parameters.AddWithValue("$tdp", totalDiscountPercent);
                            icmd.Parameters.AddWithValue("$tot", total);
                            icmd.ExecuteNonQuery();
                        }

                        using (var icmd = conn.CreateCommand())
                        {
                            icmd.Transaction = tx;
                            icmd.CommandText = @"
                                INSERT INTO ReceiptItems(ReceiptNumber, Name, Price, Quantity, DiscountPercent, Subtotal)
                                VALUES ($num, $name, $price, $qty, $disc, $sub);
                            ";
                            var pNum = icmd.CreateParameter(); pNum.ParameterName = "$num"; icmd.Parameters.Add(pNum);
                            var pName = icmd.CreateParameter(); pName.ParameterName = "$name"; icmd.Parameters.Add(pName);
                            var pPrice = icmd.CreateParameter(); pPrice.ParameterName = "$price"; icmd.Parameters.Add(pPrice);
                            var pQty = icmd.CreateParameter(); pQty.ParameterName = "$qty"; icmd.Parameters.Add(pQty);
                            var pDisc = icmd.CreateParameter(); pDisc.ParameterName = "$disc"; icmd.Parameters.Add(pDisc);
                            var pSub = icmd.CreateParameter(); pSub.ParameterName = "$sub"; icmd.Parameters.Add(pSub);

                            foreach (var it in items)
                            {
                                pNum.Value = number;
                                pName.Value = it.Name;
                                pPrice.Value = it.Price;
                                pQty.Value = it.Quantity;
                                pDisc.Value = it.DiscountPercent;
                                pSub.Value = it.Subtotal;
                                icmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }

                    // Remove legacy file after successful insert or if it already existed
                    try { File.Delete(file); } catch { /* ignore */ }
                }
                catch
                {
                    // If anything fails, leave the file so the user can investigate
                }
            }
        }

        private static decimal ParseDecimalFromCurrency(string input)
        {
            var digits = new string(input.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray());
            if (!decimal.TryParse(digits, out var value)) value = 0m;
            return value;
        }

        public int GetNextReceiptNumber()
        {
            EnsureReceiptsFolder();
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IFNULL(MAX(Number), 0) FROM Receipts";
            var result = cmd.ExecuteScalar();
            var max = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            return max + 1;
        }

        public void SaveReceipt(int number, string paymentMethod, IEnumerable<CartItem> cart, decimal subtotal, decimal gst, decimal qst, decimal totalDiscountPercent, decimal total)
        {
            EnsureReceiptsFolder();
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    INSERT INTO Receipts(Number, DateUtc, PaymentMethod, Subtotal, Gst, Qst, TotalDiscountPercent, Total)
                    VALUES ($num, $date, $method, $sub, $gst, $qst, $tdp, $tot);
                ";
                cmd.Parameters.AddWithValue("$num", number);
                cmd.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
                cmd.Parameters.AddWithValue("$method", paymentMethod);
                cmd.Parameters.AddWithValue("$sub", subtotal);
                cmd.Parameters.AddWithValue("$gst", gst);
                cmd.Parameters.AddWithValue("$qst", qst);
                cmd.Parameters.AddWithValue("$tdp", totalDiscountPercent);
                cmd.Parameters.AddWithValue("$tot", total);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    INSERT INTO ReceiptItems(ReceiptNumber, Name, Price, Quantity, DiscountPercent, Subtotal)
                    VALUES ($num, $name, $price, $qty, $disc, $sub);
                ";
                var pNum = cmd.CreateParameter(); pNum.ParameterName = "$num"; cmd.Parameters.Add(pNum);
                var pName = cmd.CreateParameter(); pName.ParameterName = "$name"; cmd.Parameters.Add(pName);
                var pPrice = cmd.CreateParameter(); pPrice.ParameterName = "$price"; cmd.Parameters.Add(pPrice);
                var pQty = cmd.CreateParameter(); pQty.ParameterName = "$qty"; cmd.Parameters.Add(pQty);
                var pDisc = cmd.CreateParameter(); pDisc.ParameterName = "$disc"; cmd.Parameters.Add(pDisc);
                var pSub = cmd.CreateParameter(); pSub.ParameterName = "$sub"; cmd.Parameters.Add(pSub);

                foreach (var ci in cart)
                {
                    pNum.Value = number;
                    pName.Value = ci.Name;
                    pPrice.Value = ci.Price;
                    pQty.Value = ci.Quantity;
                    pDisc.Value = ci.DiscountPercent;
                    pSub.Value = ci.Subtotal;
                    cmd.ExecuteNonQuery();
                }
            }

            tx.Commit();
        }

        public bool TryLoadReceipt(int number, ObservableCollection<CartItem> cart, out decimal totalDiscountPercent)
        {
            totalDiscountPercent = 0m;
            EnsureReceiptsFolder();
            using var conn = new SqliteConnection(GetConnectionString());
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TotalDiscountPercent FROM Receipts WHERE Number = $num";
                cmd.Parameters.AddWithValue("$num", number);
                var v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value)
                {
                    return false;
                }
                totalDiscountPercent = Convert.ToDecimal(v);
            }

            var items = new List<CartItem>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Name, Price, Quantity, DiscountPercent
                    FROM ReceiptItems
                    WHERE ReceiptNumber = $num
                    ORDER BY Id ASC;
                ";
                cmd.Parameters.AddWithValue("$num", number);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var price = Convert.ToDecimal(reader.GetValue(1));
                    var qty = reader.GetInt32(2);
                    var disc = Convert.ToDecimal(reader.GetValue(3));
                    items.Add(new CartItem { Name = name, Price = price, Quantity = qty, DiscountPercent = disc });
                }
            }

            cart.Clear();
            foreach (var it in items) cart.Add(it);
            return true;
        }
    }
}