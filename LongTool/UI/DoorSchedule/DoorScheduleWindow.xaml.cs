using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LongTool.Models;

namespace LongTool.UI.DoorSchedule
{
    public partial class DoorScheduleWindow : Window
    {
        public DoorScheduleWindow()
        {
            InitializeComponent();
        }

        public void SetData(IEnumerable<DoorScheduleItem> items)
        {
            List<DoorScheduleItem> data = items.ToList();

            DoorDataGrid.ItemsSource = data;

            int totalQuantity = data.Sum(x => x.Quantity);

            TotalTextBlock.Text = totalQuantity.ToString();
        }
    }
}