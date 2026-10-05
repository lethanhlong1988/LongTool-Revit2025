using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LongTool.UI.Common.SelectionDialog
{
    public partial class SelectionDialogView : Window
    {
        private readonly ISelectionProvider _provider;

        public SelectionDialogItem SelectedItem { get; private set; }

        public SelectionDialogView(ISelectionProvider provider)
        {
            InitializeComponent();

            _provider = provider;

            var items = provider
                .GetItems()
                ?.ToList()
                ?? new List<SelectionDialogItem>();

            var vm = new SelectionDialogViewModel
            {
                DialogTitle = provider.Title,
                Label = provider.Label,
                Description = provider.Description,   // ← MỚI
                Items = items
            };

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

            DataContext = vm;
        }

        private void OkButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            var vm = DataContext as SelectionDialogViewModel;

            if (vm?.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn một giá trị.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            SelectedItem = vm.SelectedItem;
            _provider.SaveSelectedItem(SelectedItem);

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