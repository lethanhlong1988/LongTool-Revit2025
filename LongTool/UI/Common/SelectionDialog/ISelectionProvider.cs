using System.Collections.Generic;

namespace LongTool.UI.Common.SelectionDialog
{
    public interface ISelectionProvider
    {
        /// <summary>
        /// Tiêu đề của dialog (VD: "Select Door")
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Nhãn câu hỏi hiển thị bên trái ComboBox (VD: "Door:")
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Dòng mô tả/yêu cầu hiển thị PHÍA TRÊN ComboBox
        /// (VD: "Hãy lựa chọn một cửa.")
        /// </summary>
        string Description { get; }

        IEnumerable<SelectionDialogItem> GetItems();

        SelectionDialogItem GetSavedItem();

        void SaveSelectedItem(SelectionDialogItem item);
    }
}