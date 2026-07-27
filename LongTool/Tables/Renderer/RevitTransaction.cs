using Autodesk.Revit.DB;
using System;

namespace LongTool.Tables.Renderer;

/// <summary>
/// Wrapper quản lý Transaction trong Revit.
/// </summary>
public sealed class RevitTransaction : IDisposable
{
    private readonly Transaction _transaction;

    private bool _started;


    public RevitTransaction(
        Document document,
        string name)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));


        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Transaction name is required.",
                nameof(name));


        _transaction =
            new Transaction(
                document,
                name);
    }



    /// <summary>
    /// Bắt đầu transaction.
    /// </summary>
    public void Start()
    {
        if (_started)
            throw new InvalidOperationException(
                "Transaction already started.");


        _transaction.Start();

        _started = true;
    }



    /// <summary>
    /// Commit thay đổi.
    /// </summary>
    public void Commit()
    {
        if (!_started)
            throw new InvalidOperationException(
                "Transaction has not started.");


        _transaction.Commit();

        _started = false;
    }



    /// <summary>
    /// Rollback thay đổi.
    /// </summary>
    public void RollBack()
    {
        if (!_started)
            return;


        _transaction.RollBack();

        _started = false;
    }



    public void Dispose()
    {
        if (_started)
        {
            _transaction.RollBack();

            _started = false;
        }


        _transaction.Dispose();
    }
}