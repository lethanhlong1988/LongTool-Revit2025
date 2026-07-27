using System.Windows;

namespace LongTool.UI.Table;

public partial class TableNameWindow : Window
{
    public string TableName =>
        TableNameTextBox.Text.Trim();

    public TableNameWindow()
    {
        InitializeComponent();

        TableNameTextBox.Focus();
        TableNameTextBox.SelectAll();
    }

    private void OkButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TableName))
        {
            MessageBox.Show(
                "Please enter a table name.",
                "LongTool");

            TableNameTextBox.Focus();

            return;
        }

        DialogResult = true;
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}