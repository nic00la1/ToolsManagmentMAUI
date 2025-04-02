using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.Services;
using ToolsManagmentMAUI.ViewModels;

namespace ToolsManagmentMAUI.ViewModels;

public class ShoppingCartViewModel : BaseViewModel
{
    private readonly ShoppingCartService _shoppingCartService;

    public ObservableCollection<ShoppingCartItem> CartItems =>
        _shoppingCartService.CartItems;

    public int TotalItems => CartItems.Sum(item => item.Quantity);
    public decimal GrandTotal => CartItems.Sum(item => item.TotalPrice);

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand CheckoutCommand { get; }

    public ShoppingCartViewModel()
    {
        _shoppingCartService = new ShoppingCartService();
        AddToCartCommand =
            new Command<Tool>(async (tool) => await AddToCartAsync(tool));
        RemoveFromCartCommand = new Command<ShoppingCartItem>(async (item) =>
            await RemoveFromCartAsync(item));
        IncreaseQuantityCommand =
            new Command<ShoppingCartItem>(async (item) =>
                await IncreaseQuantityAsync(item));
        DecreaseQuantityCommand =
            new Command<ShoppingCartItem>(async (item) =>
                await DecreaseQuantityAsync(item));
        CheckoutCommand = new Command(Checkout);
        CartItems.CollectionChanged +=
            (s, e) => OnPropertyChanged(nameof(TotalItems));
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await _shoppingCartService.LoadCartAsync();
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
    }

    private async Task AddToCartAsync(Tool tool)
    {
        await _shoppingCartService.AddToCartAsync(tool);
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
    }

    private async Task RemoveFromCartAsync(ShoppingCartItem item)
    {
        await _shoppingCartService.RemoveFromCartAsync(item);
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
    }

    private async Task IncreaseQuantityAsync(ShoppingCartItem item)
    {
        if (item != null)
        {
            item.Quantity++;
            item.TotalPrice = item.Tool.Price * item.Quantity;
            await _shoppingCartService.SaveCartAsync();
            UpdateCart();
        }
    }

    private async Task DecreaseQuantityAsync(ShoppingCartItem item)
    {
        if (item != null && item.Quantity > 1)
        {
            item.Quantity--;
            item.TotalPrice = item.Tool.Price * item.Quantity;
            await _shoppingCartService.SaveCartAsync();
            UpdateCart();
        }
    }

    private void Checkout()
    {
        // Implement checkout logic here
    }

    private void UpdateCart()
    {
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
    }
}
