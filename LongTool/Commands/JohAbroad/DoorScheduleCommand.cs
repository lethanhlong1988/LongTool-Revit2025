using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Models;
using LongTool.UI.DoorSchedule;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.ReadOnly)]
    public class DoorScheduleCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc =
                commandData.Application.ActiveUIDocument;

            Document doc =
                uiDoc.Document;

            // ========================================
            // 1. Collect all Door instances
            //    - Current Document
            //    - All Loaded Revit Links
            // ========================================

            List<FamilyInstance> doors =
                CollectAllDoors(doc);

            // ========================================
            // 2. Read Door Parameters
            // ========================================

            List<DoorScheduleItem> doorItems =
                new List<DoorScheduleItem>();

            foreach (FamilyInstance door in doors)
            {
                string jName =
                    GetTextParameter(door, "J_Name");

                string jNumber =
                    GetTextParameter(door, "J_Number");

                string symbol = string.Empty;

                if (!string.IsNullOrWhiteSpace(jName) &&
                    !string.IsNullOrWhiteSpace(jNumber))
                {
                    symbol = $"{jName}-{jNumber}";
                }
                else if (!string.IsNullOrWhiteSpace(jName))
                {
                    symbol = jName;
                }
                else if (!string.IsNullOrWhiteSpace(jNumber))
                {
                    symbol = jNumber;
                }

                string jType =
                    GetTextParameter(door, "J_Type");

                string jLocation =
                    GetTextParameter(door, "J_Location");

                string jGlass =
                    GetTextParameter(door, "J_Glass");

                string jFinish =
                    GetTextParameter(door, "J_Finish");

                string jHardware =
                    GetTextParameter(door, "J_Hardware");

                string jRemarks =
                    GetTextParameter(door, "J_Remarks");

                string jDoorThickness =
                    GetTextParameter(door, "J_DoorThickness");

                string jFrameThickness =
                    GetTextParameter(door, "J_FrameThickness");

                // ========================================
                // Door Width / Height
                // ========================================

                double width =
                    GetLengthParameterInMillimeters(
                        door,
                        "Width");

                double height =
                    GetLengthParameterInMillimeters(
                        door,
                        "Height");

                // ========================================
                // Create Schedule Item
                // ========================================

                DoorScheduleItem item =
                    new DoorScheduleItem
                    {
                        Symbol = symbol,
                        Quantity = 1,

                        Type = jType,
                        Location = jLocation,
                        Glass = jGlass,
                        Finish = jFinish,
                        Hardware = jHardware,
                        Remarks = jRemarks,

                        DoorThickness = jDoorThickness,
                        FrameThickness = jFrameThickness,

                        ShapeWidth = width,
                        ShapeHeight = height
                    };

                doorItems.Add(item);
            }

            // ========================================
            // 3. Group by 記号
            // ========================================

            List<DoorScheduleItem> groupedDoors =
                doorItems
                    .GroupBy(x => x.Symbol)
                    .Select(group => new DoorScheduleItem
                    {
                        Symbol = group.Key,
                        Quantity = group.Count(),

                        Type = group.First().Type,
                        Location = group.First().Location,
                        Glass = group.First().Glass,
                        Finish = group.First().Finish,
                        Hardware = group.First().Hardware,
                        Remarks = group.First().Remarks,

                        ShapeWidth = group.First().ShapeWidth,
                        ShapeHeight = group.First().ShapeHeight,

                        DoorThickness = group.First().DoorThickness,
                        FrameThickness = group.First().FrameThickness
                    })
                    .OrderBy(x => x.Symbol)
                    .ToList();

            // ========================================
            // 4. Show Door Schedule
            // ========================================

            DoorScheduleWindow window =
                new DoorScheduleWindow();

            window.SetData(groupedDoors);

            window.Show();

            return Result.Succeeded;
        }

        // ========================================
        // Collect all Doors
        //
        // Includes:
        // 1. Current Document
        // 2. All Loaded Revit Links
        // ========================================

        private static List<FamilyInstance> CollectAllDoors(
            Document currentDoc)
        {
            List<FamilyInstance> allDoors =
                new List<FamilyInstance>();

            // ========================================
            // A. Doors in Current Document
            // ========================================

            List<FamilyInstance> currentDoors =
                new FilteredElementCollector(currentDoc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .OfType<FamilyInstance>()
                    .ToList();

            allDoors.AddRange(currentDoors);

            // ========================================
            // B. Doors in Revit Links
            // ========================================

            List<RevitLinkInstance> linkInstances =
                new FilteredElementCollector(currentDoc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Cast<RevitLinkInstance>()
                    .ToList();

            foreach (RevitLinkInstance linkInstance in linkInstances)
            {
                Document? linkDoc =
                    linkInstance.GetLinkDocument();

                // Link is unloaded or unavailable
                if (linkDoc == null)
                {
                    continue;
                }

                List<FamilyInstance> linkedDoors =
                    new FilteredElementCollector(linkDoc)
                        .OfCategory(BuiltInCategory.OST_Doors)
                        .WhereElementIsNotElementType()
                        .OfType<FamilyInstance>()
                        .ToList();

                allDoors.AddRange(linkedDoors);
            }

            return allDoors;
        }

        // ========================================
        // Get Text Parameter
        // ========================================

        private static string GetTextParameter(
            Element element,
            string parameterName)
        {
            Parameter? parameter =
                element.LookupParameter(parameterName);

            if (parameter == null)
            {
                return string.Empty;
            }

            return parameter.AsString() ?? string.Empty;
        }

        // ========================================
        // Get Length Parameter → mm
        // ========================================

        private static double GetLengthParameterInMillimeters(
            Element element,
            string parameterName)
        {
            Parameter? parameter =
                element.LookupParameter(parameterName);

            if (parameter == null)
            {
                return 0;
            }

            if (parameter.StorageType != StorageType.Double)
            {
                return 0;
            }

            double valueInFeet =
                parameter.AsDouble();

            return UnitUtils.ConvertFromInternalUnits(
                valueInFeet,
                UnitTypeId.Millimeters);
        }
    }
}