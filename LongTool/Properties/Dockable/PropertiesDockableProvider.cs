using Autodesk.Revit.UI;
using LongTool.Properties.Services;
using LongTool.Properties.Views;

namespace LongTool.Properties.Dockable;

public class PropertiesDockableProvider
    : IDockablePaneProvider
{
    public static PropertiesView Instance { get; private set; }


    private ParameterUpdateExternalEvent?
        _updateHandler;


    private ExternalEvent?
        _updateExternalEvent;


    public void SetupDockablePane(
        DockablePaneProviderData data)
    {
        _updateHandler =
            new ParameterUpdateExternalEvent();


        _updateExternalEvent =
            ExternalEvent.Create(
                _updateHandler);


        Instance =
            new PropertiesView();


        Instance.ViewModel
            .SetUpdateAction(
                RequestUpdate);


        data.FrameworkElement =
            Instance;


        data.InitialState =
            new DockablePaneState
            {
                DockPosition =
                    DockPosition.Right
            };
    }


    private void RequestUpdate(
        Autodesk.Revit.DB.Document document,
        LongTool.Storage.Models.ElementData data,
        LongTool.Properties.Models.PropertiesParameterItem item)
    {
        if (_updateHandler == null)
        {
            return;
        }


        if (_updateExternalEvent == null)
        {
            return;
        }


        _updateHandler.RequestUpdate(
            document,
            data,
            item);


        _updateExternalEvent.Raise();
    }
}