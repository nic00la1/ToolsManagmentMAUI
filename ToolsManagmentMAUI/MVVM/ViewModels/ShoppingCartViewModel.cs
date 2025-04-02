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
    private readonly AlertService _alertService;
    private string _promoCode;
    private decimal _discount;
    private bool _isPromoCodeApplied;

    public ObservableCollection<ShoppingCartItem> CartItems =>
        _shoppingCartService.CartItems;

    public int TotalItems => CartItems.Sum(item => item.Quantity);

    public decimal GrandTotal =>
        CartItems.Sum(item => item.TotalPrice) - _discount;

    public decimal DiscountAmount => _discount;

    public string PromoCode
    {
        get => _promoCode;
        set => SetProperty(ref _promoCode, value);
    }

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand ApplyPromoCodeCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand ClearCartCommand { get; }

    public ShoppingCartViewModel()
    {
        _shoppingCartService = new ShoppingCartService();
        _alertService = new AlertService();
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
        ApplyPromoCodeCommand =
            new Command(async () => await ApplyPromoCodeAsync());
        CheckoutCommand = new Command(Checkout);
        ClearCartCommand = new Command(async () => await ClearCartAsync());
        CartItems.CollectionChanged +=
            (s, e) => OnPropertyChanged(nameof(TotalItems));
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await _shoppingCartService.LoadCartAsync();
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(DiscountAmount));
    }

    private async Task AddToCartAsync(Tool tool)
    {
        await _shoppingCartService.AddToCartAsync(tool);
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(DiscountAmount));
    }

    private async Task RemoveFromCartAsync(ShoppingCartItem item)
    {
        await _shoppingCartService.RemoveFromCartAsync(item);
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(DiscountAmount));
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

    private async Task ApplyPromoCodeAsync()
    {
        if (_isPromoCodeApplied)
        {
            await _alertService.ShowErrorMessageAsync(
                "Kod rabatowy zosta³ ju¿ u¿yty.");
            return;
        }

        // Example logic for applying a discount based on the promo code
        if (PromoCode == "DISCOUNT10")
        {
            _discount =
                CartItems.Sum(item => item.TotalPrice) * 0.10m; // 10% discount
            _isPromoCodeApplied = true;
            await _alertService.ShowSuccessMessageAsync(
                "10% Zni¿ki w³aœnie wlecia³o, na twoje produkty.");
        } else if (PromoCode == "DISCOUNT20")
        {
            _discount =
                CartItems.Sum(item => item.TotalPrice) * 0.20m; // 20% discount
            _isPromoCodeApplied = true;
            await _alertService.ShowSuccessMessageAsync(
                "20% Zni¿ki w³aœnie wlecia³o, na twoje produkty.");
        } else
        {
            _discount = 0;
            await _alertService.ShowErrorMessageAsync(
                "Niepoprawny kod rabatowy!");
        }

        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(DiscountAmount));
    }

    private async Task ClearCartAsync()
    {
        bool confirm = await _alertService.ShowConfirmationAsync(
            "Potwierdzenie",
            "Czy na pewno chcesz usun¹æ wszystkie przedmioty z koszyka?");
        if (confirm)
        {
            _shoppingCartService.CartItems.Clear();
            await _shoppingCartService.SaveCartAsync();
            UpdateCart();
        }
    }

    private void Checkout()
    {
        // Implement checkout logic here

        // Reset promo code and related properties after checkout
        PromoCode = string.Empty;
        _discount = 0;
        _isPromoCodeApplied = false;
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(DiscountAmount));
    }

    private void UpdateCart()
    {
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(DiscountAmount));
    }
}
