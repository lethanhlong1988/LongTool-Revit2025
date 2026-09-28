using Autodesk.Revit.DB;
using LongTool.Services.LineStyle;
using LongTool.Settings;
using System.Linq;
using System.Windows;

namespace LongTool.UI.LineStyle
{
    public partial class LineStyleSelectionView : Window
    {
        private readonly LineStyleService _lineStyleService;
        private readonly SettingsStorageService _settingsStorageService;

        public GraphicsStyle SelectedLineStyle { get; private set; }

        public LineStyleSelectionView(Document doc)
        {
            InitializeComponent();

            _lineStyleService =
                new LineStyleService();

            _settingsStorageService =
                new SettingsStorageService();

            LoadLineStyles(doc);
        }

        // ============================================================
        // LOAD LINE STYLE
        // ============================================================

        private void LoadLineStyles(Document doc)
        {
            var lineStyles =
                _lineStyleService
                    .GetDetailLineStyles(doc);

            foreach (GraphicsStyle lineStyle in lineStyles)
            {
                LineStyleComboBox.Items.Add(lineStyle);
            }

            if (LineStyleComboBox.Items.Count == 0)
            {
                return;
            }

            // ========================================================
            // ĐỌC STYLE ĐÃ LƯU
            // ========================================================

            LineStyleSettings savedSettings =
                _settingsStorageService
                    .LoadLineStyleSettings();

            if (savedSettings != null &&
                savedSettings.HasValue)
            {
                GraphicsStyle savedStyle =
                    lineStyles.FirstOrDefault(
                        gs =>
                            gs.Id.Value ==
                            savedSettings.LineStyleId);

                if (savedStyle != null)
                {
                    LineStyleComboBox.SelectedItem =
                        savedStyle;

                    return;
                }
            }

            // ========================================================
            // NẾU CHƯA CÓ STYLE ĐÃ LƯU
            // → CHỌN STYLE ĐẦU TIÊN
            // ========================================================

            LineStyleComboBox.SelectedIndex = 0;
        }

        // ============================================================
        // OK
        // ============================================================

        private void OkButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectedLineStyle =
                LineStyleComboBox.SelectedItem
                as GraphicsStyle;

            if (SelectedLineStyle == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn một Line Style.",
                    "Select Line Style",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // ============================================================
            // LƯU LINE STYLE ĐÃ CHỌN
            // ============================================================

            LineStyleSettings settings =
                new LineStyleSettings
                {
                    LineStyleId =
                        SelectedLineStyle.Id.Value,

                    LineStyleName =
                        SelectedLineStyle.Name
                };

            _settingsStorageService
                .SaveLineStyleSettings(settings);

            // ============================================================
            // ĐÓNG WINDOW
            // ============================================================

            DialogResult = true;
            Close();
        }

        // ============================================================
        // CANCEL
        // ============================================================

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}