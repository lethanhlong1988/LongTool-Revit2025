using LongTool.Inspect.Models;
using LongTool.Storage.Models;
using System.Collections.Generic;

namespace LongTool.Inspect.Services;

public class InspectorService
{
    public List<InspectGroup> Inspect(
        ElementData element)
    {
        var groups = new List<InspectGroup>();
        var usedParameters = new HashSet<string>();


        var general = new InspectGroup
        {
            Name = "General"
        };

        general.Parameters.Add(
            new ParameterData
            {
                Name = "Id",
                DisplayValue = element.Id.ToString()
            });

        general.Parameters.Add(
            new ParameterData
            {
                Name = "Category",
                DisplayValue = element.Category
            });


        general.Parameters.Add(
            new ParameterData
            {
                Name = "Name",
                DisplayValue = element.Name
            });

        general.Parameters.Add(
            new ParameterData
            {
                Name = "UniqueId",
                DisplayValue = element.UniqueId
            });

        groups.Add(general);

        var identity = new InspectGroup
        {
            Name = "Identity"
        };

        foreach (var parameter in element.Parameters)
        {
            if (parameter.Name == "Mark" ||
                parameter.Name == "Comments")
            {
                identity.Parameters.Add(parameter);

                usedParameters.Add(parameter.Name);
            }
        }

        if (identity.Parameters.Count > 0)
        {
            groups.Add(identity);
        }

        var dimensions = new InspectGroup
        {
            Name = "Dimensions"
        };

        foreach (var parameter in element.Parameters)
        {
            if (parameter.Unit == "mm" ||
                parameter.Unit == "m" ||
                parameter.Unit == "sq.m" ||
                parameter.Unit == "cu.m")
            
            {
                dimensions.Parameters.Add(parameter);

                usedParameters.Add(parameter.Name);
            }
        }

        if (dimensions.Parameters.Count > 0)
        {
            groups.Add(dimensions);
        }

        var parameters = new InspectGroup
        {
            Name = "Parameters"
        };

        foreach (var parameter in element.Parameters)
        {
            if (!usedParameters.Contains(parameter.Name))
            {
                parameters.Parameters.Add(parameter);
            }
        }

        groups.Add(parameters);


        return groups;
    }
}