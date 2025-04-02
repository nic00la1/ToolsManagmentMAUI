using ToolsManagmentMAUI.ViewModels;

public static class ViewModelLocator
{
    static ViewModelLocator()
    {
        ShoppingCartViewModel = new ShoppingCartViewModel();
    }

    public static ShoppingCartViewModel ShoppingCartViewModel { get; }
}
