using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LongTool.UI.Common.SelectionDialog
{
    public class SelectionDialogViewModel : INotifyPropertyChanged
    {
        public string DialogTitle { get; set; }
        public string Label { get; set; }

        // MỚI
        public string Description { get; set; }

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