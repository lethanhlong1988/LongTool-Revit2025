using System;
using System.IO;
using System.Text.Json;

namespace LongTool.Settings
{
    public class SettingsStorageService
    {
        private readonly string _settingsFolder;
        private readonly string _lineStyleSettingsFile;

        public SettingsStorageService()
        {
            string appDataFolder =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);

            _settingsFolder =
                Path.Combine(
                    appDataFolder,
                    "LongTool");

            _lineStyleSettingsFile =
                Path.Combine(
                    _settingsFolder,
                    "LineStyleSettings.json");
        }

        // ============================================================
        // LƯU LINE STYLE SETTINGS
        // ============================================================

        public void SaveLineStyleSettings(
            LineStyleSettings settings)
        {
            if (settings == null)
                return;

            Directory.CreateDirectory(
                _settingsFolder);

            string json =
                JsonSerializer.Serialize(
                    settings,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            File.WriteAllText(
                _lineStyleSettingsFile,
                json);
        }

        // ============================================================
        // ĐỌC LINE STYLE SETTINGS
        // ============================================================

        public LineStyleSettings LoadLineStyleSettings()
        {
            if (!File.Exists(
                    _lineStyleSettingsFile))
            {
                return null;
            }

            try
            {
                string json =
                    File.ReadAllText(
                        _lineStyleSettingsFile);

                return JsonSerializer.Deserialize<LineStyleSettings>(
                    json);
            }
            catch
            {
                return null;
            }
        }
    }
}