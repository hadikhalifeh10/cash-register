using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System;
using System.Collections.Generic;
using cashregister.Services;
using cashregister.Common;

namespace cashregister.ViewModel
{
    public class TransactionsViewModel
    {
        public ObservableCollection<TransactionItem> Receipts { get; } = new();
        public decimal MoneyMade { get; private set; }
        private readonly ReceiptService _service = new();

        private List<ReceiptRecord> _all = new();
        private List<ReceiptRecord> _filtered = new();

        // Paging
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set { if (value <= 0) value = 10; if (_pageSize == value) return; _pageSize = value; ApplyFiltersAndPage(resetPage: true); }
        }
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            private set { if (value == _currentPage) return; _currentPage = value; RebuildPage(); }
        }
        public int TotalPages { get; private set; }

        // Filters: number range
        public string NumberFromText { get; set; } = string.Empty;
        public string NumberToText { get; set; } = string.Empty;

        // Filters: date range
        public ObservableCollection<int> Years { get; } = new();
        public ObservableCollection<int> Months { get; } = new(Enumerable.Range(1, 12).ToList());
        private int? _startYear; public int? StartYear { get => _startYear; set { _startYear = value; } }
        private int? _startMonth; public int? StartMonth { get => _startMonth; set { _startMonth = value; } }
        public string StartDayText { get; set; } = string.Empty;
        private int? _endYear; public int? EndYear { get => _endYear; set { _endYear = value; } }
        private int? _endMonth; public int? EndMonth { get => _endMonth; set { _endMonth = value; } }
        public string EndDayText { get; set; } = string.Empty;

        // Type filter
        public ObservableCollection<string> ReceiptTypes { get; } = new(new[] { "All", "Regular", "Refunded", "S.I.R", "F.R" });
        private string _selectedReceiptType = "All";
        public string SelectedReceiptType { get => _selectedReceiptType; set { if (_selectedReceiptType == value) return; _selectedReceiptType = value; ApplyFiltersAndPage(resetPage: true); } }

        public RelayCommand CopyToCartCommand { get; }
        public RelayCommand DeleteReceiptCommand { get; }
        public RelayCommand RefundItemCommand { get; }
        public RelayCommand OpenHtmlCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public RelayCommand ApplyNumberFilterCommand { get; }
        public RelayCommand ApplyDateFilterCommand { get; }
        public RelayCommand ClearFiltersCommand { get; }
        public RelayCommand PrevPageCommand { get; }
        public RelayCommand NextPageCommand { get; }

        public TransactionsViewModel()
        {
            Refresh();
            CopyToCartCommand = new RelayCommand(p => CopyToCart(p as TransactionItem));
            DeleteReceiptCommand = new RelayCommand(p => DeleteReceipt(p as TransactionItem));
            RefundItemCommand = new RelayCommand(p => RefundItem(p as Model.CartItem));
            OpenHtmlCommand = new RelayCommand(p => OpenHtml(p as TransactionItem));
            InitializeCommand = new RelayCommand(_ => InitializeAll());

            ApplyNumberFilterCommand = new RelayCommand(_ => ApplyFiltersAndPage(true));
            ApplyDateFilterCommand = new RelayCommand(_ => ApplyFiltersAndPage(true));
            ClearFiltersCommand = new RelayCommand(_ => { NumberFromText = NumberToText = string.Empty; StartYear = StartMonth = EndYear = EndMonth = null; StartDayText = EndDayText = string.Empty; _selectedReceiptType = "All"; ApplyFiltersAndPage(true); });
            PrevPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) CurrentPage--; });
            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) CurrentPage++; });
        }

        private void Refresh()
        {
            _all = _service.GetReceiptHistory(1000).ToList();
            // build year options from data
            Years.Clear();
            var years = _all.Select(r => r.DateUtc.Year).Distinct().OrderBy(y => y);
            foreach (var y in years) Years.Add(y);
            ApplyFiltersAndPage(resetPage: true);
        }

        private void ApplyFiltersAndPage(bool resetPage)
        {
            IEnumerable<ReceiptRecord> query = _all;

            // number range
            if (int.TryParse(NumberFromText, out var nfrom))
            {
                query = query.Where(r => r.Number >= nfrom);
            }
            if (int.TryParse(NumberToText, out var nto))
            {
                query = query.Where(r => r.Number <= nto);
            }

            // date range
            DateTime? start = null, end = null;
            if (StartYear.HasValue && StartMonth.HasValue && int.TryParse(StartDayText, out var sd))
            {
                start = new DateTime(StartYear.Value, Math.Clamp(StartMonth.Value, 1, 12), Math.Clamp(sd, 1, 31));
            }
            if (EndYear.HasValue && EndMonth.HasValue && int.TryParse(EndDayText, out var ed))
            {
                end = new DateTime(EndYear.Value, Math.Clamp(EndMonth.Value, 1, 12), Math.Clamp(ed, 1, 31)).AddDays(1).AddTicks(-1); // inclusive end
            }
            if (start.HasValue)
            {
                query = query.Where(r => r.DateUtc >= start.Value);
            }
            if (end.HasValue)
            {
                query = query.Where(r => r.DateUtc <= end.Value);
            }

            // type filter
            query = SelectedReceiptType switch
            {
                "Regular" => query.Where(r => !r.IsRefund && r.RefundStatus == 0),
                "Refunded" => query.Where(r => r.RefundStatus == 1),
                "S.I.R" => query.Where(r => r.IsRefund && r.Items.Count == 1),
                "F.R" => query.Where(r => r.IsRefund && r.Items.Count == 0),
                _ => query
            };

            _filtered = query.ToList();
            MoneyMade = _filtered.Sum(h => h.Total);

            // paging
            TotalPages = Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)PageSize));
            if (resetPage) _currentPage = 1;
            if (_currentPage > TotalPages) _currentPage = TotalPages;
            RebuildPage();
        }

        private void RebuildPage()
        {
            Receipts.Clear();
            var pageItems = _filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            foreach (var h in pageItems)
            {
                string indicator;
                if (h.RefundStatus == 1)
                {
                    indicator = " (REFUNDED)";
                }
                else if (h.RefundStatus == 2)
                {
                    var code = (h.Items.Count == 1) ? "S.I.R" : "F.R";
                    var orderRef = h.RefundOfNumber.HasValue ? $" of #{h.RefundOfNumber.Value}" : string.Empty;
                    indicator = $" ({code}{orderRef})";
                }
                else
                {
                    indicator = string.Empty;
                }

                Receipts.Add(new TransactionItem
                {
                    Number = h.Number,
                    HeaderText = $"#{h.Number} - {h.DateUtc.ToLocalTime():yyyy-MM-dd HH:mm}{indicator}",
                    Items = new ObservableCollection<Model.CartItem>(h.Items)
                });
            }
        }

        private void InitializeAll()
        {
            _service.InitializeAll();
            Refresh();
        }

        private void CopyToCart(TransactionItem? tx)
        {
            if (tx == null) return;
            if (Application.Current?.MainWindow?.DataContext is MainViewModel vm)
            {
                vm.Cart.Clear();
                foreach (var it in tx.Items)
                {
                    vm.Cart.Add(new Model.CartItem { Name = it.Name, Price = it.Price, Quantity = it.Quantity, DiscountPercent = it.DiscountPercent, RefundStatus = it.RefundStatus });
                }
                vm.TotalDiscountPercent = 0m;
                vm.StatusMessage = $"Loaded receipt #{tx.Number} into cart";
            }
        }

        private void DeleteReceipt(TransactionItem? tx)
        {
            if (tx == null) return;
            _service.DeleteReceiptNoShift(tx.Number);
            Refresh();
        }

        private void RefundItem(Model.CartItem? item)
        {
            if (item == null) return;
            // Prevent double refund from UI
            if (item.RefundStatus != 0) return;

            var container = Receipts.FirstOrDefault(r => r.Items.Contains(item));
            if (container == null) return;
            _service.RefundSingleItem(container.Number, item.Name, item.Price, item.Quantity, item.DiscountPercent);
            Refresh();
        }

        private void OpenHtml(TransactionItem? tx)
        {
            if (tx == null) return;
            var path = _service.EnsureHtmlReceipt(tx.Number);
            if (File.Exists(path))
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true // open with default browser
                    };
                    Process.Start(psi);
                }
                catch
                {
                    // ignore open errors
                }
            }
        }
    }

    public class TransactionItem
    {
        public int Number { get; set; }
        public string HeaderText { get; set; } = string.Empty;
        public ObservableCollection<Model.CartItem> Items { get; set; } = new();
    }
}
