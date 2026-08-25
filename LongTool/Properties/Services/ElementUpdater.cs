using Autodesk.Revit.DB;
using LongTool.Storage.Models;
using System.Diagnostics;
using System.Linq;
using LongTool.Properties.Models;

namespace LongTool.Properties.Services;

public class ElementUpdater
{
    public bool Update(
        Document document,
        ElementData data,
        PropertiesParameterItem item)
    {
        Debug.WriteLine(
            $"Updating Parameter: {item.Name} | " +
            $"IsTypeParameter: {item.IsTypeParameter}");

        Debug.WriteLine(
            "ElementUpdater Started");


        ElementId id =
            new ElementId(data.Id);


        Element? element =
            document.GetElement(id);


        if (element == null)
        {
            Debug.WriteLine(
                "Element NOT Found");

            return false;
        }


        Debug.WriteLine(
            $"Element Found: {element.Name}");

        ElementType? type =
            document.GetElement(
                element.GetTypeId())
            as ElementType;


        if (type != null)
        {
            Debug.WriteLine(
                $"Element Type: {type.Name}");


            Parameter? typeParameter =
                type.LookupParameter(
                    "Analytic Construction");


            if (typeParameter != null)
            {
                Debug.WriteLine(
                    $"Type Parameter Value: " +
                    $"{typeParameter.AsString()}");
            }
            else
            {
                Debug.WriteLine(
                    "Analytic Construction NOT FOUND on Type");
            }
        }


        Debug.WriteLine(
            "Before LookupParameter");


        Parameter? parameter = null;


        if (item.IsTypeParameter)
        {
            ElementType? elementType =
                document.GetElement(
                    element.GetTypeId())
                as ElementType;


            if (elementType != null)
            {
                Debug.WriteLine(
                    $"Element Type: {elementType.Name}");

                parameter =
                    elementType.LookupParameter(
                        item.Name);


                if (parameter != null)
                {
                    Debug.WriteLine(
                        $"Parameter found on Type: {parameter.Definition.Name}");
                }
            }
        }
        else
        {
            parameter =
                element.LookupParameter(
                    item.Name);


            if (parameter != null)
            {
                Debug.WriteLine(
                    $"Parameter found on Instance: {parameter.Definition.Name}");
            }
        }


        Debug.WriteLine(
            "After LookupParameter");

        Debug.WriteLine(
            $"Parameter Owner Id: " +
            $"{parameter?.Element.Id}");


        Debug.WriteLine(
            $"Instance Id: " +
            $"{element.Id}");


        Debug.WriteLine(
            $"Type Id: " +
            $"{element.GetTypeId()}");

        Debug.WriteLine(
            $"Parameter Owner ElementId: " +
            $"{parameter?.Element.Id}");


        Debug.WriteLine(
            $"Instance ElementId: " +
            $"{element.Id}");


        Debug.WriteLine(
            $"Type ElementId: " +
            $"{element.GetTypeId()}");



        if (parameter == null)
        {
            Debug.WriteLine(
                "Parameter NOT Found");

            return false;
        }


        Debug.WriteLine(
            $"Parameter Found: {parameter.Definition.Name}");


        if (parameter.IsReadOnly)
        {
            Debug.WriteLine(
                "Parameter is ReadOnly");

            return false;
        }


        ParameterData? parameterData =
            data.Parameters
                .FirstOrDefault(
                    p => p.Name ==
                         parameter.Definition.Name);

        if (parameterData == null)
        {
            Debug.WriteLine(
                "ParameterData NOT Found");

            return false;
        }


        Debug.WriteLine(
            $"StorageType: {parameter.StorageType}");

        Debug.WriteLine(
            $"Parameter Type: {parameter.Definition.GetDataType()}");

        Debug.WriteLine(
            $"Parameter Data Type Id: " +
            $"{parameter.Definition.GetDataType().TypeId}");

        Debug.WriteLine(
            $"Parameter Definition Type: " +
            $"{parameter.Definition.GetType().FullName}");

        Debug.WriteLine(
            $"Parameter Id: " +
            $"{parameter.Id}");

        if (parameter.Id.Value < 0)
        {
            try
            {
                BuiltInParameter builtInParameter =
                    (BuiltInParameter)parameter.Id.Value;

                Debug.WriteLine(
                    $"BuiltInParameter: " +
                    $"{builtInParameter}");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(
                    $"BuiltInParameter conversion failed: " +
                    $"{ex.Message}");
            }
        }

        Element? parameterElement =
            document.GetElement(
                parameter.Id);

        Debug.WriteLine(
            $"Parameter Element: " +
            $"{parameterElement?.GetType().FullName}");

        Debug.WriteLine(
            $"Is Analytic Construction: " +
            $"{parameter.Id == new ElementId(
                BuiltInParameter.ANALYTIC_CONSTRUCTION_LOOKUP_TABLE)}");

        Debug.WriteLine(
            $"Parameter Name: " +
            $"{parameter.Definition.Name}");

        if (parameter.Definition is InternalDefinition internalDefinition)
        {
            Debug.WriteLine(
                $"Parameter Group Id: " +
                $"{internalDefinition.GetGroupTypeId()}");
        }

        Debug.WriteLine(
            $"Parameter IsReadOnly: {parameter.IsReadOnly}");

        Debug.WriteLine(
            $"Parameter HasValue: {parameter.HasValue}");

        Debug.WriteLine(
            $"Parameter Value: {parameter.AsValueString()}");

        Debug.WriteLine(
            $"New Value: {parameterData.DisplayValue}");


        try
        {
            using Transaction transaction =
                new Transaction(
                    document,
                    "Update Parameter");


            transaction.Start();


            bool changed = false;


            switch (parameter.StorageType)
            {
                case StorageType.String:

                    Debug.WriteLine(
                        $"Before Set: {parameter.AsString()}");


                    changed =
                        parameter.Set(
                            parameterData.DisplayValue);


                    Debug.WriteLine(
                        $"Set Result: {changed}");


                    Debug.WriteLine(
                        $"After Set: {parameter.AsString()}");


                    break;


                case StorageType.Double:

                    if (parameterData.NumericValue.HasValue)
                    {
                        changed =
                            parameter.Set(
                                parameterData.NumericValue.Value);
                    }

                    break;


                case StorageType.Integer:

                    if (int.TryParse(
                        parameterData.DisplayValue,
                        out int integerValue))
                    {
                        changed =
                            parameter.Set(
                                integerValue);
                    }

                    break;


                default:

                    Debug.WriteLine(
                        $"Unsupported StorageType: " +
                        $"{parameter.StorageType}");

                    transaction.RollBack();

                    return false;
            }


            if (!changed)
            {
                transaction.RollBack();

                Debug.WriteLine(
                    "Parameter value was not changed.");

                return false;
            }


            transaction.Commit();


            Debug.WriteLine(
                "Parameter Updated");


            Debug.WriteLine(
                $"After Commit - Parameter Owner: " +
                $"{parameter.Element.Id}");


            Debug.WriteLine(
                $"After Commit - Type Id: " +
                $"{element.GetTypeId()}");


            Debug.WriteLine(
                $"After Commit - Value: " +
                $"{parameter.AsString()}");


            return true;
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine(
                $"Update failed: {ex.Message}");

            return false;
        }
    }
}