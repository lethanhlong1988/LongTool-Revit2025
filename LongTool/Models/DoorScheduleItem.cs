namespace LongTool.Models
{
    public class DoorScheduleItem
    {
        // 記号
        public string Symbol { get; set; } = string.Empty;

        // 数量
        public int Quantity { get; set; }

        // 型式
        public string Type { get; set; } = string.Empty;

        // 場所
        public string Location { get; set; } = string.Empty;

        // ガラス
        public string Glass { get; set; } = string.Empty;

        // 仕上
        public string Finish { get; set; } = string.Empty;

        // 金物
        public string Hardware { get; set; } = string.Empty;

        // 備考
        public string Remarks { get; set; } = string.Empty;

        // 扉厚
        public string DoorThickness { get; set; } = string.Empty;

        // 枠厚
        public string FrameThickness { get; set; } = string.Empty;

        // 形状・寸法
        // 後で Door Legend を表示するための領域
        public double ShapeWidth { get; set; }

        public double ShapeHeight { get; set; }
    }
}