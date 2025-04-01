using ToolsManagmentMAUI.Services;
using ToolsManagmentMAUI.ViewModels;

namespace ToolsManagmentMAUI.MVVM.Views;

public partial class AddToolPage : ContentPage
{
    public AddToolPage()
    {
        InitializeComponent();
        BindingContext =
            new AddToolViewModel(new ToolService(), new AlertService());
    }
}
