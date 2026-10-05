using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LongTool.UI.Common.SelectionDialog;

namespace LongTool.UI.Common.FormDialog
{
    public class FormFieldViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Key định danh field, dùng để lấy kết quả.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Nhãn (VD: "Line Style:")
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Mô tả/yêu cầu (VD: "Hãy chọn một Line Style.")
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách item hiển thị trong ComboBox.
        /// </summary>
        public List<SelectionDialogItem> Items { get; set; }

        private SelectionDialogItem _selectedItem;
        public SelectionDialogItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(name));
        }
    }
}