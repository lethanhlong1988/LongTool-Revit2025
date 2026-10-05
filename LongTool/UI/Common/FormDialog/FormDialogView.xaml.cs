using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LongTool.UI.Common.SelectionDialog;

namespace LongTool.UI.Common.FormDialog
{
    public partial class FormDialogView : Window
    {
        private readonly List<FormFieldViewModel> _fields;

        /// <summary>
        /// Kết quả sau khi user bấm OK.
        /// Key = field.Key, Value = FormFieldResult.
        /// </summary>
        public Dictionary<string, FormFieldResult> Results { get; private set; }

        /// <summary>
        /// Tạo form từ nhiều provider.
        /// </summary>
        /// <param name="title">Tiêu đề dialog</param>
        /// <param name="fields">
        /// Danh sách (key, provider) — mỗi provider là 1 yêu cầu nhập liệu.
        /// </param>
        public FormDialogView(
            string title,
            IEnumerable<(string Key, ISelectionProvider Provider)> fields)
        {
            InitializeComponent();

            _fields = new List<FormFieldViewModel>();

            foreach (var (key, provider) in fields)
            {
                var items = provider.GetItems()?.ToList()
                            ?? new List<SelectionDialogItem>();

                var vm = new FormFieldViewModel
                {
                    Key = key,
                    Label = provider.Label,
                    Description = provider.Description,
                    Items = items
                };

                // Pre-select từ settings
                var saved = provider.GetSavedItem();
                if (saved != null)
                {
                    vm.SelectedItem = items
                        .FirstOrDefault(x => x.Id == saved.Id);
                }

                if (vm.SelectedItem == null && items.Count > 0)
                {
                    vm.SelectedItem = items[0];
                }

                _fields.Add(vm);
            }

            DataContext = new FormDialogViewModel
            {
                DialogTitle = title,
                Fields = _fields
            };
        }

        private void OkButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            // Validate tất cả field đều có chọn
            var missing = _fields
                .Where(f => f.SelectedItem == null)
                .ToList();

            if (missing.Count > 0)
            {
                var names = string.Join(
                    "\n",
                    missing.Select(f => $"- {f.Label}"));

                MessageBox.Show(
                    $"Vui lòng chọn giá trị cho:\n{names}",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Đóng gói kết quả
            Results = _fields.ToDictionary(
                f => f.Key,
                f => new FormFieldResult
                {
                    Key = f.Key,
                    Item = f.SelectedItem
                });

            // Lưu settings cho từng provider
            // (nếu cần, dùng lại _fields gốc — xem bên dưới)

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}