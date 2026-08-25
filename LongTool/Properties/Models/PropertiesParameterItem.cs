namespace LongTool.Properties.Models;
using LongTool.Core.MVVM;


public class PropertiesParameterItem
    : BaseViewModel
{
    public string Name { get; set; } = "";

    public bool IsTypeParameter { get; set; }

    private string _value = "";


    public string Value
    {
        get => _value;

        set
        {
            if (_value == value)
                return;

            _value = value;

            OnPropertyChanged();
        }
    }

    public string Unit { get; set; } = "";
}