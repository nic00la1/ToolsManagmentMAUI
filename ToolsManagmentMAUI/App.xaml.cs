using ToolsManagmentMAUI;
using ToolsManagmentMAUI.Services;

namespace ToolsManagmentMAUI;

public partial class App : Application
{
    private readonly ShoppingCartService _shoppingCartService;

    public App()
    {
        InitializeComponent();

        _shoppingCartService = new ShoppingCartService();

        MainPage = new AppShell();

        // Load the cart state when the app starts
        LoadCartState();
    }

    protected override void OnStart()
    {
        // Handle when your app starts
    }

    protected override void OnSleep()
    {
        // Handle when your app sleeps
        SaveCartState();
    }

    protected override void OnResume()
    {
        // Handle when your app resumes
    }

    private async void SaveCartState()
    {
        await _shoppingCartService.SaveCartAsync();
    }

    private async void LoadCartState()
    {
        await _shoppingCartService.LoadCartAsync();
    }
}
