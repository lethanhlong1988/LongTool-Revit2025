using Autodesk.Revit.DB;
using LongTool.Core.MVVM;
using LongTool.Properties.Models;
using LongTool.Properties.Services;
using LongTool.Storage.Constants;
using LongTool.Storage.Models;
using LongTool.Storage.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;


namespace LongTool.Properties.ViewModels;


public class PropertiesViewModel
    : BaseViewModel
{

    private string _category = "-";
    private ElementData? _currentElement;
    private Document? _document;

    private Action<
    Document,
    ElementData,
    PropertiesParameterItem>? _updateAction;


    public string Category
    {
        get => _category;

        set
        {
            if (_category == value)
                return;

            _category = value;

            OnPropertyChanged();
        }
    }

    private string _family = "-";


    public string Family
    {
        get => _family;

        set
        {
            if (_family == value)
                return;

            _family = value;

            OnPropertyChanged();
        }
    }

    private string _type = "-";


    public string Type
    {
        get => _type;

        set
        {
            if (_type == value)
                return;

            _type = value;

            OnPropertyChanged();
        }
    }

    private string _level = "-";


    public string Level
    {
        get => _level;

        set
        {
            if (_level == value)
                return;

            _level = value;

            OnPropertyChanged();
        }
    }


    private string _width = "-";


    public string Width
    {
        get => _width;

        set
        {
            if (_width == value)
                return;

            _width = value;

            OnPropertyChanged();
        }
    }

    //public string Height { get; set; } = "-";
    private string _height = "-";


    public string Height
    {
        get => _height;

        set
        {
            if (_height == value)
                return;

            _height = value;

            OnPropertyChanged();
        }
    }

    //public string Thickness { get; set; } = "-";
    private string _thickness = "-";


    public string Thickness
    {
        get => _thickness;

        set
        {
            if (_thickness == value)
                return;

            _thickness = value;

            OnPropertyChanged();
        }
    }


    private ObservableCollection<PropertiesParameterItem>
    _parameters = new();


    public ObservableCollection<PropertiesParameterItem>
        Parameters
    {
        get => _parameters;

        set
        {
            if (_parameters == value)
                return;

            _parameters = value;

            OnPropertyChanged();
        }
    }

    public void SetUpdateAction(
    Action<
        Document,
        ElementData,
        PropertiesParameterItem> updateAction)
    {
        _updateAction = updateAction;
    }


    // Tạo đổi tượng giả tạm thời hiện thị
    public PropertiesViewModel()
    {
        Parameters =
            new ObservableCollection<PropertiesParameterItem>();
    }

    //Lấy dữ liệu từ ElementData và chuyển đổi sang PropertiesViewModel
    public void Load(
    Document document,
    ElementData element)
    {
        _document = document;

        _currentElement = element;

        Category = element.Category;

        Family = GetParameterValue(
            element,
            PropertyKeys.Family);

        Type = GetParameterValue(
            element,
            PropertyKeys.Type);

        Level = GetParameterValue(
            element,
            PropertyKeys.Level);


        Width = GetParameterValue(
            element,
            PropertyKeys.Width);

        Height = GetParameterValue(
            element,
            PropertyKeys.Height);

        Thickness = GetParameterValue(
            element,
            PropertyKeys.Thickness);


        Parameters.Clear();


        foreach (var parameter in element.Parameters)
        {
            if (PropertyKeys.HeaderProperties
                .Contains(parameter.Name))
            {
                continue;
            }

            System.Diagnostics.Debug.WriteLine(
                $"Property: {parameter.Name} | " +
                $"TypeParameter: {parameter.IsTypeParameter}");

            var item = new PropertiesParameterItem
            {
                Name = parameter.Name,
                Value = parameter.DisplayValue,
                Unit = parameter.Unit,
                IsTypeParameter = parameter.IsTypeParameter
            };


            item.PropertyChanged += Parameter_PropertyChanged;


            Parameters.Add(item);
        }
    }

    // Xủ lý sự kiện khi một tham số thay đổi giá trị
    private void Parameter_PropertyChanged(
    object? sender,
    PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(
            PropertiesParameterItem.Value))
        {
            return;
        }


        if (sender is not PropertiesParameterItem item)
        {
            return;
        }


        if (_currentElement == null)
        {
            return;
        }

        System.Diagnostics.Debug.WriteLine(
            $"Changed Parameter: {item.Name} | " +
            $"IsTypeParameter: {item.IsTypeParameter}");


        if (_document == null)
        {
            return;
        }


        var parameter =
            _currentElement.Parameters
                .FirstOrDefault(
                    p => p.Name == item.Name);


        if (parameter == null)
        {
            return;
        }


        parameter.DisplayValue =
            item.Value;


        System.Diagnostics.Debug.WriteLine(
            $"Parameter Changed: " +
            $"{parameter.Name} = " +
            $"{parameter.DisplayValue}");


        if (_updateAction != null)
        {
            _updateAction(
                _document,
                _currentElement,
                item);
        }
    }

    // Lấy giá trị của một tham số từ ElementData dựa trên tên tham số
    private static string GetParameterValue(
    ElementData element,
    string key)
    {
        return element.Parameters
            .FirstOrDefault(p => p.Name == key)
            ?.DisplayValue
            ?? "-";
    }

}