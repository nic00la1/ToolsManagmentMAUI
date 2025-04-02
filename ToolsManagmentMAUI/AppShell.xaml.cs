using ToolsManagmentMAUI.MVVM.Views;
using ToolsManagmentMAUI.ViewModels;

namespace ToolsManagmentMAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(ShoppingCartPage),
            typeof(ShoppingCartPage));
    }
}
