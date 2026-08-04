using Autodesk.Revit.DB;
using LongTool.Storage.Models;
using LongTool.Storage.Services;
using System;
using System.Linq;

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

            UniqueId = element.UniqueId,

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

        ElementType? elementType =
    document.GetElement(element.GetTypeId()) as ElementType;


        if (elementType != null)
        {
            foreach (Parameter parameter in elementType.Parameters)
            {
                ParameterData? parameterData =
                    ParameterConverter.Convert(parameter);


                if (parameterData != null &&
                    !data.Parameters.Any(p => p.Name == parameterData.Name))
                {
                    data.Parameters.Add(parameterData);
                }
            }
        }


        return data;
    }
}