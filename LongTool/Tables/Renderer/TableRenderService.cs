using LongTool.Tables.Engine;
using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Renderer;

/// <summary>
/// Service cấp cao chịu trách nhiệm render Table vào Revit.
/// </summary>
public sealed class TableRenderService
{
    private readonly RevitRenderContext _context;


    private readonly ITableRenderer _renderer;



    public TableRenderService(
        RevitRenderContext context)
    {
        _context =
            context ??
            throw new ArgumentNullException(nameof(context));


        _renderer =
            new RevitTableRenderer(_context);
    }



    /// <summary>
    /// Render một bảng vào Revit.
    /// </summary>
    public void Render(Table table)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));


        TableLayout layout =
            table.CreateLayout();


        using (RevitTransaction transaction =
              new RevitTransaction(
                  _context.Document,
                  "Render Table"))
        {
            transaction.Start();

            _renderer.Render(layout);

            transaction.Commit();
        }
    }
}