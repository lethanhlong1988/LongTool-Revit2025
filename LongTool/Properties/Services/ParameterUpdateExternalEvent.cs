using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Storage.Models;
using LongTool.Storage.Services;
using System.Diagnostics;
using LongTool.Properties.Models;

namespace LongTool.Properties.Services;

public class ParameterUpdateExternalEvent
    : IExternalEventHandler
{
    private Document? _document;

    private ElementData? _elementData;

    private PropertiesParameterItem? _parameterItem;


    private readonly ElementUpdater _updater =
        new ElementUpdater();


    public void RequestUpdate(
        Document document,
        ElementData elementData,
        PropertiesParameterItem parameterItem)
    {
        _document = document;

        _elementData = elementData;

        _parameterItem = parameterItem;
    }


    public void Execute(
        UIApplication app)
    {
        if (_document == null)
        {
            if (_parameterItem == null)
            {
                Debug.WriteLine(
                    "ExternalEvent: ParameterItem is null");

                return;
            }

            Debug.WriteLine(
                "ExternalEvent: Document is null");

            return;
        }


        if (_elementData == null)
        {
            Debug.WriteLine(
                "ExternalEvent: ElementData is null");

            return;
        }


        Debug.WriteLine(
            "ExternalEvent Execute");


        bool success =
            _updater.Update(
                _document,
                _elementData,
                _parameterItem);


        if (!success)
        {
            Debug.WriteLine(
                "Revit update failed.");

            return;
        }


        Debug.WriteLine(
            "Revit update succeeded.");


        JsonStorageService.Update(
            _document,
            _elementData);


        Debug.WriteLine(
            "Storage updated.");
    }


    public string GetName()
    {
        return "LongTool Parameter Update";
    }
}