using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.Services;
using ToolsManagmentMAUI.ViewModels;

namespace ToolsManagmentMAUI.ViewModels
{
    public class ShoppingCartViewModel : BaseViewModel
    {
        private readonly ShoppingCartService _shoppingCartService;
        private readonly AlertService _alertService;
        private readonly OrderService _orderService;
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
            _orderService = new OrderService(_shoppingCartService);
            _shoppingCartService.AvailableToolsChanged += OnAvailableToolsChanged;


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
                (s, e) => UpdateCart();
            InitializeAsync();


        }

        private async void InitializeAsync()
        {
            await _shoppingCartService.LoadCartAsync();
            UpdateCart();
        }

        private async Task AddToCartAsync(Tool tool)
        {
            try
            {
                await _shoppingCartService.AddToCartAsync(tool);
                UpdateCart();
            }
            catch (InvalidOperationException ex)
            {
                await _alertService.ShowErrorMessageAsync(ex.Message);
            }
        }

        private async Task RemoveFromCartAsync(ShoppingCartItem item)
        {
            await _shoppingCartService.RemoveFromCartAsync(item);
            UpdateCart();
        }

        private async Task IncreaseQuantityAsync(ShoppingCartItem item)
        {
            if (item != null)
            {
                try
                {
                    await _shoppingCartService.IncreaseQuantityAsync(item);
                    UpdateCart();
                }
                catch (InvalidOperationException ex)
                {
                    await _alertService.ShowErrorMessageAsync(ex.Message);
                }
            }
        }

        private async Task DecreaseQuantityAsync(ShoppingCartItem item)
        {
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                item.TotalPrice = item.Tool.Price * item.Quantity;

                // Zwi�ksz ilo�� dost�pnych narz�dzi w sklepie
                var tool = _shoppingCartService.GetAvailableTool(item.Tool.Id);
                if (tool != null)
                {
                    tool.Quantity++;
                }

                await _shoppingCartService.SaveCartAsync();
                UpdateCart();
            }
        }

        private async Task ApplyPromoCodeAsync()
        {
            if (_isPromoCodeApplied)
            {
                await _alertService.ShowErrorMessageAsync(
                    "Kod rabatowy zosta� ju� u�yty.");
                return;
            }

            // Example logic for applying a discount based on the promo code
            if (PromoCode == "DISCOUNT10")
            {
                _isPromoCodeApplied = true;
                await _alertService.ShowSuccessMessageAsync(
                    "10% Zni�ki w�a�nie wlecia�o, na twoje produkty.");
            }
            else if (PromoCode == "DISCOUNT20")
            {
                _isPromoCodeApplied = true;
                await _alertService.ShowSuccessMessageAsync(
                    "20% Zni�ki w�a�nie wlecia�o, na twoje produkty.");
            }
            else
            {
                _discount = 0;
                await _alertService.ShowErrorMessageAsync(
                    "Niepoprawny kod rabatowy!");
                return;
            }

            UpdateCart();
        }

        private async Task ClearCartAsync()
        {
            bool confirm = await _alertService.ShowConfirmationAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usun�� wszystkie przedmioty z koszyka?");
            if (confirm)
            {
                _shoppingCartService.CartItems.Clear();
                await _shoppingCartService.SaveCartAsync();
                UpdateCart();
            }
        }

        private void Checkout()
        {
            // Process order
            _orderService.ProcessOrder(CartItems);

            // Update available tools and clear cart items
            foreach (var item in CartItems.ToList())
            {
                var tool = _shoppingCartService.GetAvailableTool(item.Tool.Id);
                if (tool != null)
                {
                    tool.Quantity -= item.Quantity;
                    if (tool.Quantity <= 0)
                    {
                        tool.Quantity = 0; // Set tool quantity to 0
                        _shoppingCartService.MarkToolAsUnavailable(tool.Id);
                    }
                }
                _shoppingCartService.CartItems.Remove(item);
            }

            // Notify about the changes
            OnAvailableToolsChanged(this, EventArgs.Empty);

            // Save changes to the cart
            _shoppingCartService.SaveCartAsync();

            // Reset promo code and related properties after checkout
            PromoCode = string.Empty;
            _discount = 0;
            _isPromoCodeApplied = false;
            OnPropertyChanged(nameof(GrandTotal));
            OnPropertyChanged(nameof(DiscountAmount));
            OnPropertyChanged(nameof(TotalItems));
        }

        private void UpdateCart()
        {
            if (_isPromoCodeApplied)
            {
                if (PromoCode == "DISCOUNT10")
                    _discount =
                        CartItems.Sum(item => item.TotalPrice) *
                        0.10m; // 10% discount
                else if (PromoCode == "DISCOUNT20")
                    _discount =
                        CartItems.Sum(item => item.TotalPrice) *
                        0.20m; // 20% discount
            }
            else
                _discount = 0;

            OnPropertyChanged(nameof(TotalItems));
            OnPropertyChanged(nameof(GrandTotal));
            OnPropertyChanged(nameof(DiscountAmount));
        }

        public int GetAvailableQuantity(int toolId)
        {
            return _shoppingCartService.GetAvailableQuantity(toolId);
        }

        private void OnAvailableToolsChanged(object sender, EventArgs e)
        {
            // Notify MainPageViewModel about the changes
            MessagingCenter.Send(this, "AvailableToolsChanged");
        }
    }
}