using ToolsManagmentMAUI.Services;
using ToolsManagmentMAUI.ViewModels;

namespace ToolsManagmentMAUI.MVVM.Views;

public partial class MainPage : ContentPage
{
    private readonly ToolService _toolService;

    public MainPage()
    {
        InitializeComponent();
        _toolService = new ToolService();
        BindingContext =
            new MainPageViewModel(_toolService, new AlertService());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((MainPageViewModel)BindingContext).LoadToolsAsync();
    }
}
