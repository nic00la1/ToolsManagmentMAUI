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
    private List<Tool> _availableTools = new List<Tool>
    {
        new Tool { Id = 1, Name = "M³otek", Quantity = 10, Price = 50 },
        new Tool { Id = 2, Name = "Œrubokrêt", Quantity = 20, Price = 30 },
        // Dodaj wiêcej narzêdzi wed³ug potrzeb
    };

    public ShoppingCartService()
    {
        _filePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder
                    .LocalApplicationData), CartFileName);
        _cartItems = new ObservableCollection<ShoppingCartItem>();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadCartAsync();
    }

    public ObservableCollection<ShoppingCartItem> CartItems => _cartItems;

    public async Task AddToCartAsync(Tool tool)
    {
        ShoppingCartItem? existingItem =
            _cartItems.FirstOrDefault(item => item.Tool.Id == tool.Id);
        if (existingItem != null)
        {
            if (existingItem.Quantity < tool.Quantity)
            {
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.Quantity * tool.Price;
            }
            else
            {
                // Inform the user that they cannot add more items than available in the store
                throw new InvalidOperationException("Nie mo¿na dodaæ wiêcej narzêdzi ni¿ dostêpnych w sklepie.");
            }
        }
        else
        {
            if (tool.Quantity > 0)
            {
                _cartItems.Add(new ShoppingCartItem
                {
                    Tool = tool,
                    Quantity = 1,
                    TotalPrice = tool.Price
                });
            }
            else
            {
                // Inform the user that they cannot add an item that is not available in the store
                throw new InvalidOperationException("Nie mo¿na dodaæ narzêdzia, które nie jest dostêpne w sklepie.");
            }
        }

        // Zmniejsz iloœæ dostêpnych narzêdzi w sklepie
        var availableTool = _availableTools.FirstOrDefault(t => t.Id == tool.Id);
        if (availableTool != null)
        {
            availableTool.Quantity--;
        }

        await SaveCartAsync();
    }

    public async Task IncreaseQuantityAsync(ShoppingCartItem item)
    {
        if (item != null)
        {
            var tool = _availableTools.FirstOrDefault(t => t.Id == item.Tool.Id);
            if (tool != null && item.Quantity < tool.Quantity)
            {
                item.Quantity++;
                item.TotalPrice = item.Quantity * tool.Price;

                // Zmniejsz iloœæ dostêpnych narzêdzi w sklepie
                tool.Quantity--;

                await SaveCartAsync();
            }
            else
            {
                // Inform the user that they cannot increase the quantity beyond available stock
                throw new InvalidOperationException("Nie mo¿na zwiêkszyæ iloœci powy¿ej dostêpnej w sklepie.");
            }
        }
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
            ObservableCollection<ShoppingCartItem>? items =
                JsonSerializer
                    .Deserialize<ObservableCollection<ShoppingCartItem>>(json);
            if (items != null)
            {
                _cartItems.Clear();
                foreach (ShoppingCartItem item in items) _cartItems.Add(item);
            }
        }
    }

    public int GetAvailableQuantity(int toolId)
    {
        var tool = _availableTools.FirstOrDefault(t => t.Id == toolId);
        return tool?.Quantity ?? 0;
    }

    public Tool GetAvailableTool(int toolId)
    {
        return _availableTools.FirstOrDefault(t => t.Id == toolId);
    }

    public void RemoveTool(int toolId)
    {
        var tool = _availableTools.FirstOrDefault(t => t.Id == toolId);
        if (tool != null)
        {
            _availableTools.Remove(tool);
        }
    }
}