using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LongTool.UI.Common.FormDialog
{
    public class FormDialogViewModel : INotifyPropertyChanged
    {
        public string DialogTitle { get; set; }

        /// <summary>
        /// Danh sách các field con (mỗi field = 1 ISelectionProvider).
        /// </summary>
        public List<FormFieldViewModel> Fields { get; set; }

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