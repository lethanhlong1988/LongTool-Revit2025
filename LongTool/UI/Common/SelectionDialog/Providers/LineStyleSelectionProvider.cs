using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LongTool.Services.LineStyle;
using LongTool.Settings;

namespace LongTool.UI.Common.SelectionDialog.Providers
{
    public class LineStyleSelectionProvider : ISelectionProvider
    {
        private readonly Document _doc;
        private readonly LineStyleService _lineStyleService;
        private readonly SettingsStorageService _settingsStorageService;

        public string Title => "Select Line Style";
        public string Label => "Line Style:";

        // ⭐ NỘI DUNG MÔ TẢ — THAY ĐỔI TÙY THEO NGỮ CẢNH
        public string Description => "Hãy lựa chọn một loại nét.";

        public LineStyleSelectionProvider(Document doc)
        {
            _doc = doc;
            _lineStyleService = new LineStyleService();
            _settingsStorageService = new SettingsStorageService();
        }

        public IEnumerable<SelectionDialogItem> GetItems()
        {
            var styles = _lineStyleService
                .GetDetailLineStyles(_doc);

            return styles.Select(gs => new SelectionDialogItem
            {
                Id = gs.Id.Value,
                Name = gs.Name,
                Tag = gs
            });
        }

        public SelectionDialogItem GetSavedItem()
        {
            var settings = _settingsStorageService
                .LoadLineStyleSettings();

            if (settings == null || !settings.HasValue)
                return null;

            return new SelectionDialogItem
            {
                Id = settings.LineStyleId,
                Name = settings.LineStyleName
            };
        }

        public void SaveSelectedItem(SelectionDialogItem item)
        {
            var settings = new LineStyleSettings
            {
                LineStyleId = item.Id,
                LineStyleName = item.Name
            };

            _settingsStorageService
                .SaveLineStyleSettings(settings);
        }
    }
}