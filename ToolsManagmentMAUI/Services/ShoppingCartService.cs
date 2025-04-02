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
        _filePath = Path.Combine(FileSystem.AppDataDirectory, CartFileName);
        _cartItems = new ObservableCollection<ShoppingCartItem>();
        InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        await LoadCartAsync();
    }

    public ObservableCollection<ShoppingCartItem> CartItems
    {
        get => _cartItems;
        private set => _cartItems = value;
    }

    public async Task AddToCartAsync(Tool tool)
    {
        ShoppingCartItem? existingItem =
            _cartItems.FirstOrDefault(i => i.Tool.Name == tool.Name);
        if (existingItem != null)
            existingItem.Quantity += 1;
        else
            _cartItems.Add(new ShoppingCartItem { Tool = tool, Quantity = 1 });
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
            ObservableCollection<ShoppingCartItem>? cartItems =
                JsonSerializer
                    .Deserialize<ObservableCollection<ShoppingCartItem>>(json);
            if (cartItems != null) _cartItems = cartItems;
        }
    }
}
