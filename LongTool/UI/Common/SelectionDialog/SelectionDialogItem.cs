namespace LongTool.UI.Common.SelectionDialog
{
    /// <summary>
    /// Item hiển thị trong ComboBox.
    /// Có thể wrap bất kỳ object nào (GraphicsStyle, Level, FamilySymbol...).
    /// </summary>
    public class SelectionDialogItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Object gốc (có thể là GraphicsStyle, Level...).
        /// Dùng để cast lại sau khi user chọn.
        /// </summary>
        public object Tag { get; set; }

        public override string ToString() => Name;
    }
}