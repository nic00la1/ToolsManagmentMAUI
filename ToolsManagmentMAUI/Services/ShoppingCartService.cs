using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ToolsManagmentMAUI.Models;

namespace ToolsManagmentMAUI.Services;

public class ShoppingCartService
{
    private const string CartFileName = "shopping_cart.json";
    private readonly string _filePath;
    private ObservableCollection<ShoppingCartItem> _cartItems;

    public ShoppingCartService()
    {
        _filePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder
                    .LocalApplicationData), CartFileName);
        _cartItems = new ObservableCollection<ShoppingCartItem>();
        // Initialize asynchronously
        Task.Run(async () => await InitializeAsync()).Wait();
    }

    private async Task InitializeAsync()
    {
        if (File.Exists(_filePath))
        {
            string json = await File.ReadAllTextAsync(_filePath);
            _cartItems =
                JsonSerializer
                    .Deserialize<
                        ObservableCollection<ShoppingCartItem>>(json) ??
                new ObservableCollection<ShoppingCartItem>();
        }
    }

    public ObservableCollection<ShoppingCartItem> CartItems
    {
        get => _cartItems;
        set
        {
            _cartItems = value;
            Task.Run(async () => await SaveCartAsync()).Wait();
        }
    }

    public async Task AddToCartAsync(Tool tool)
    {
        ShoppingCartItem? existingItem =
            _cartItems.FirstOrDefault(item => item.Tool.Id == tool.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
            existingItem.TotalPrice = existingItem.Quantity * tool.Price;
        } else
            _cartItems.Add(new ShoppingCartItem
                { Tool = tool, Quantity = 1, TotalPrice = tool.Price });

        await SaveCartAsync();
    }

    public async Task RemoveFromCartAsync(ShoppingCartItem item)
    {
        _cartItems.Remove(item);
        await SaveCartAsync();
    }

    public async Task SaveCartAsync()
    {
        string json = JsonSerializer.Serialize(_cartItems);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task LoadCartAsync()
    {
        if (File.Exists(_filePath))
        {
            string json = await File.ReadAllTextAsync(_filePath);
            _cartItems =
                JsonSerializer
                    .Deserialize<
                        ObservableCollection<ShoppingCartItem>>(json) ??
                new ObservableCollection<ShoppingCartItem>();
        }
    }
}
