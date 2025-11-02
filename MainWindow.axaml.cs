using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using InventorySystems_Week6.Models;

namespace InventorySystems_Week6;

public partial class MainWindow : Window
{ 
    // ====== Domænemodeller (samme som før, men med InventoryLocation) ======
    public class Item
    {
        public string Name { get; set; } = "";
        public decimal PricePerUnit { get; set; }
        public uint InventoryLocation { get; set; } // 1=a, 2=b, 3=c
        public override string ToString() => $"{Name}: {PricePerUnit} kr. (X={InventoryLocation})";
    }

    public class OrderLine
    {
        public Item Item { get; set; } = new();
        public int Quantity { get; set; }
        public decimal LineTotal => Item.PricePerUnit * Quantity;
    }

    public class Order
    {
        public string Customer { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
        public ObservableCollection<OrderLine> Lines { get; set; } = new();
        public decimal TotalPrice => Calc();

        private decimal Calc()
        {
            decimal sum = 0;
            foreach (var l in Lines) sum += l.LineTotal;
            return sum;
        }
    }

    // ====== Data og robot ======
    public ObservableCollection<Order> QueuedOrders { get; } = new();
    public ObservableCollection<Order> ProcessedOrders { get; } = new();
    public decimal TotalRevenue { get; private set; }
    private readonly ItemSorterRobot _robot = new();

    // ====== Constructor ======
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Testdata fra opgaven
        var item1 = new Item { Name = "M3 screw", PricePerUnit = 1m, InventoryLocation = 1 };
        var item2 = new Item { Name = "M3 nut", PricePerUnit = 1.5m, InventoryLocation = 2 };
        var item3 = new Item { Name = "pen", PricePerUnit = 1m, InventoryLocation = 3 };

        var order1 = new Order
        {
            Customer = "Ramanda",
            Time = DateTime.Now - TimeSpan.FromDays(2),
            Lines =
            {
                new OrderLine { Item = item1, Quantity = 1 },
                new OrderLine { Item = item2, Quantity = 2 },
                new OrderLine { Item = item3, Quantity = 1 },
            }
        };

        var order2 = new Order
        {
            Customer = "Totoro",
            Time = DateTime.Now,
            Lines =
            {
                new OrderLine { Item = item2, Quantity = 2 },
            }
        };

        QueuedOrders.Add(order1);
        QueuedOrders.Add(order2);
    }

    private void Log(string s)
    {
        StatusMessages.Text += s + Environment.NewLine;
    }

    // ====== Knaphandler ======
    public async void ProcessNext_OnClick(object? sender, RoutedEventArgs e)
    {
        if (QueuedOrders.Count == 0)
        {
            Log("No queued orders.");
            return;
        }

        var o = QueuedOrders[0];
        Log($"Processing order for {o.Customer} ...");

        foreach (var line in o.Lines)
        {
            for (int i = 0; i < line.Quantity; i++)
            {
                Log($"Picking up {line.Item.Name} from X={line.Item.InventoryLocation} ...");
                _robot.PickUp(line.Item.InventoryLocation);
                await Task.Delay(9500); // vent ca. 10 sek pr. bevægelse
            }
        }

        QueuedOrders.RemoveAt(0);
        ProcessedOrders.Add(o);
        TotalRevenue += o.TotalPrice;

        DataContext = null;
        DataContext = this;

        Log("Order done. Conveyor moves the shipment box away.");
        Log($"Revenue now: {TotalRevenue:C}");
    }
}
