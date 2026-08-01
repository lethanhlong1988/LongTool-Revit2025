using Autodesk.Revit.DB;
using System;

namespace LongTool.Core.Views;

public sealed class ViewCoordinateSystem
{
    public View View { get; }


    /// <summary>
    /// Trục X theo hướng ngang của View.
    /// </summary>
    public XYZ XAxis { get; }


    /// <summary>
    /// Trục Y theo hướng lên của View.
    /// </summary>
    public XYZ YAxis { get; }


    /// <summary>
    /// Pháp tuyến hướng nhìn của View.
    /// </summary>
    public XYZ Normal { get; }


    public ViewCoordinateSystem(
        View view)
    {
        View =
            view ??
            throw new ArgumentNullException(nameof(view));


        XAxis =
            view.RightDirection;


        YAxis =
            view.UpDirection;


        Normal =
            view.ViewDirection;
    }



    public XYZ ToWorld(
        double x,
        double y,
        double z = 0)
    {
        return
            View.Origin
            + XAxis * x
            + YAxis * y
            + Normal * z;
    }



    public override string ToString()
    {
        return
            $"View: {View.Name}\n" +
            $"Type: {View.ViewType}\n\n" +

            $"X Axis:\n{XAxis}\n\n" +

            $"Y Axis:\n{YAxis}\n\n" +

            $"Normal:\n{Normal}";
    }
}