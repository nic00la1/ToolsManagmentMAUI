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
    private ObservableCollection<ShoppingCartItem> _cartItems;

    public ObservableCollection<ShoppingCartItem> CartItems
    {
        get => _cartItems;
        set
        {
            SetProperty(ref _cartItems, value);
            OnPropertyChanged(nameof(TotalItems));
        }
    }

    public int TotalItems => CartItems.Sum(item => item.Quantity);

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand CheckoutCommand { get; }

    public ShoppingCartViewModel()
    {
        _shoppingCartService = new ShoppingCartService();
        CartItems = _shoppingCartService.CartItems;
        AddToCartCommand =
            new Command<Tool>(async (tool) => await AddToCartAsync(tool));
        RemoveFromCartCommand = new Command<ShoppingCartItem>(async (item) =>
            await RemoveFromCartAsync(item));
        CheckoutCommand = new Command(Checkout);
    }

    private async Task AddToCartAsync(Tool tool)
    {
        await _shoppingCartService.AddToCartAsync(tool);
        CartItems =
            new ObservableCollection<ShoppingCartItem>(_shoppingCartService
                .CartItems);
        OnPropertyChanged(nameof(TotalItems));
    }

    private async Task RemoveFromCartAsync(ShoppingCartItem item)
    {
        await _shoppingCartService.RemoveFromCartAsync(item);
        CartItems =
            new ObservableCollection<ShoppingCartItem>(_shoppingCartService
                .CartItems);
        OnPropertyChanged(nameof(TotalItems));
    }

    private void Checkout()
    {
        // Implement checkout logic here
    }
}
