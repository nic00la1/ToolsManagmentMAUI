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

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand CheckoutCommand { get; }

    public ShoppingCartViewModel()
    {
        _shoppingCartService = new ShoppingCartService();
        AddToCartCommand =
            new Command<Tool>(async (tool) => await AddToCartAsync(tool));
        RemoveFromCartCommand = new Command<ShoppingCartItem>(async (item) =>
            await RemoveFromCartAsync(item));
        CheckoutCommand = new Command(Checkout);
        CartItems.CollectionChanged +=
            (s, e) => OnPropertyChanged(nameof(TotalItems));
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await _shoppingCartService.LoadCartAsync();
        OnPropertyChanged(nameof(TotalItems));
    }

    private async Task AddToCartAsync(Tool tool)
    {
        await _shoppingCartService.AddToCartAsync(tool);
        OnPropertyChanged(nameof(TotalItems));
    }

    private async Task RemoveFromCartAsync(ShoppingCartItem item)
    {
        await _shoppingCartService.RemoveFromCartAsync(item);
        OnPropertyChanged(nameof(TotalItems));
    }

    private void Checkout()
    {
        // Implement checkout logic here
    }
}
