namespace LongTool.UI.Common.FormDialog
{
    /// <summary>
    /// Kết quả của 1 field trong form.
    /// </summary>
    public class FormFieldResult
    {
        /// <summary>
        /// Key định danh field (VD: "LineStyle", "Door", "Legend").
        /// Dùng để lấy kết quả trong command.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Item user đã chọn.
        /// </summary>
        public SelectionDialog.SelectionDialogItem Item { get; set; }

        /// <summary>
        /// Object gốc (Tag) để cast về GraphicsStyle, FamilyInstance...
        /// </summary>
        public object Value => Item?.Tag;
    }
}