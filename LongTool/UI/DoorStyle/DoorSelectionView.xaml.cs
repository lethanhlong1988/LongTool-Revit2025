using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LongTool.UI.DoorStyle;

public partial class DoorSelectionView : Window
{
    /// <summary>
    /// Item hiển thị trong ComboBox.
    /// </summary>
    public class DoorOption
    {
        public long ElementId { get; set; }

        /// <summary>
        /// Chuỗi hiển thị trong ComboBox: "Symbol — Type (W×H)".
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        public override string ToString() => DisplayName;
    }

    public DoorOption? SelectedDoor { get; private set; }

    public DoorSelectionView(IEnumerable<DoorOption> doors)
    {
        InitializeComponent();

        List<DoorOption> list = doors.ToList();

        DoorComboBox.ItemsSource = list;

        if (list.Count > 0)
            DoorComboBox.SelectedIndex = 0;
    }

    private void OkButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DoorComboBox.SelectedItem is not DoorOption option)
        {
            MessageBox.Show(
                "Vui lòng chọn một Door.",
                "Door Board",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        SelectedDoor = option;
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