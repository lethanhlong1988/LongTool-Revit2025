using System;

namespace LongTool.Settings
{
    public class LineStyleSettings
    {
        // ============================================================
        // LINE STYLE ĐƯỢC CHỌN LẦN CUỐI
        // ============================================================

        public long LineStyleId { get; set; }

        public string LineStyleName { get; set; }

        // ============================================================
        // KIỂM TRA CÓ DỮ LIỆU HAY CHƯA
        // ============================================================

        public bool HasValue =>
            LineStyleId != 0 &&
            !string.IsNullOrWhiteSpace(LineStyleName);
    }
}