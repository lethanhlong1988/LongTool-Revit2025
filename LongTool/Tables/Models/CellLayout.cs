using LongTool.Tables.Models;

namespace LongTool.Tables.Models;

public sealed class CellLayout
{
    /// <summary>
    /// Cell gốc trong Table.
    /// </summary>
    public TableCell Cell { get; set; }


    /// <summary>
    /// Vị trí Row trong Table.
    /// </summary>
    public int Row { get; set; }


    /// <summary>
    /// Vị trí Column trong Table.
    /// </summary>
    public int Column { get; set; }


    /// <summary>
    /// Tọa độ X.
    /// </summary>
    public double X { get; set; }


    /// <summary>
    /// Tọa độ Y.
    /// </summary>
    public double Y { get; set; }


    /// <summary>
    /// Chiều rộng.
    /// </summary>
    public double Width { get; set; }


    /// <summary>
    /// Chiều cao.
    /// </summary>
    public double Height { get; set; }
}