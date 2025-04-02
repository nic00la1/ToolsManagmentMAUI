using ToolsManagmentMAUI.ViewModels;

namespace ToolsManagmentMAUI.MVVM.Views;

public partial class ShoppingCartPage : ContentPage
{
    public ShoppingCartPage()
    {
        InitializeComponent();
        BindingContext = ViewModelLocator.ShoppingCartViewModel;
    }
}
