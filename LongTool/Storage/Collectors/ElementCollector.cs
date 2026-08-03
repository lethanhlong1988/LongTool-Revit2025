using Autodesk.Revit.DB;
using LongTool.Storage.Models;
using LongTool.Storage.Services;
using System;

namespace LongTool.Storage.Collectors;

public static class ElementCollector
{
    public static ElementData Collect(
        Document document,
        ElementId elementId)
    {
        Element element = document.GetElement(elementId);

        if (element == null)
        {
            throw new InvalidOperationException(
                "Element not found.");
        }


        var data = new ElementData
        {
            Id = element.Id.Value,

            Category = element.Category?.Name
                       ?? string.Empty,

            Name = element.Name
        };


        foreach (Parameter parameter in element.Parameters)
        {
            ParameterData? parameterData =
                ParameterConverter.Convert(parameter);


            if (parameterData != null)
            {
                data.Parameters.Add(parameterData);
            }
        }


        return data;
    }
}