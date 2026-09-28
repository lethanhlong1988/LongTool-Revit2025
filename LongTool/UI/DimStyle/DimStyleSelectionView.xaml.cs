using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;

namespace LongTool.UI.DimStyle
{
    public partial class DimStyleSelectionView : Window
    {
        // ============================================================
        // KẾT QUẢ DIM STYLE ĐƯỢC CHỌN
        // ============================================================

        public DimensionType SelectedDimStyle { get; private set; }

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public DimStyleSelectionView(Document doc)
        {
            InitializeComponent();

            LoadDimStyles(doc);

            DimStyleListBox.Focus();

            if (DimStyleListBox.Items.Count > 0)
            {
                DimStyleListBox.SelectedIndex = 0;
            }
        }

        // ============================================================
        // LOAD LINEAR DIM STYLES TỪ REVIT
        // ============================================================

        private void LoadDimStyles(Document doc)
        {
            List<DimensionType> dimStyles =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(DimensionType))
                    .Cast<DimensionType>()
                    .Where(x =>
                        x.StyleType ==
                        DimensionStyleType.Linear)
                    .OrderBy(x => x.Name)
                    .ToList();

            DimStyleListBox.ItemsSource = dimStyles;
        }

        // ============================================================
        // OK
        // ============================================================

        private void OkButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DimensionType selected =
                DimStyleListBox.SelectedItem as DimensionType;

            if (selected == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn một Dim Style.",
                    "LongTool - Dim Style",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            SelectedDimStyle = selected;

            DialogResult = true;
        }

        // ============================================================
        // CANCEL
        // ============================================================

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectedDimStyle = null;

            DialogResult = false;
        }
    }
}
