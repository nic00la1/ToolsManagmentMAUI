using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.MVVM.Views;
using ToolsManagmentMAUI.Services;

namespace ToolsManagmentMAUI.ViewModels;

public class MainPageViewModel : BindableObject
{
    private readonly ToolService _toolService;
    private readonly AlertService _alertService;

    public ObservableCollection<Tool> Tools { get; set; }
    public ICommand LoadToolsCommand { get; }
    public ICommand DeleteToolCommand { get; }
    public ICommand NavigateToAddToolCommand { get; }
    public ICommand NavigateToDetailsCommand { get; }

    public MainPageViewModel()
    {
        _toolService = new ToolService();
        _alertService = new AlertService();
        _toolService.ToolAdded += OnToolAdded;
        _toolService.PropertyChanged += OnToolsChanged;
        Tools = new ObservableCollection<Tool>();
        LoadToolsCommand = new Command(async () => await LoadToolsAsync());
        DeleteToolCommand =
            new Command<Tool>(async (tool) => await DeleteToolAsync(tool));
        NavigateToAddToolCommand =
            new Command(async () => await NavigateToAddToolAsync());
        NavigateToDetailsCommand = new Command<Tool>(async (tool) =>
            await NavigateToDetailsAsync(tool));
        LoadToolsCommand.Execute(null);
    }

    public MainPageViewModel(ToolService toolService, AlertService alertService)
    {
        _toolService = toolService;
        _alertService = alertService;
        _toolService.ToolAdded += OnToolAdded;
        _toolService.PropertyChanged += OnToolsChanged;
        Tools = new ObservableCollection<Tool>();
        LoadToolsCommand = new Command(async () => await LoadToolsAsync());
        DeleteToolCommand =
            new Command<Tool>(async (tool) => await DeleteToolAsync(tool));
        NavigateToAddToolCommand =
            new Command(async () => await NavigateToAddToolAsync());
        NavigateToDetailsCommand = new Command<Tool>(async (tool) =>
            await NavigateToDetailsAsync(tool));
        LoadToolsCommand.Execute(null);
    }

    private async void OnToolAdded(object sender, EventArgs e)
    {
        await LoadToolsAsync();
    }

    private void OnToolsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToolService.Tools))
        {
            Tools.Clear();
            foreach (Tool tool in _toolService.Tools) Tools.Add(tool);
        }
    }

    public async Task LoadToolsAsync()
    {
        await _toolService.LoadToolsAsync();
        Tools.Clear();
        foreach (Tool tool in _toolService.Tools) Tools.Add(tool);
    }

    private async Task DeleteToolAsync(Tool tool)
    {
        bool confirm = await _alertService.ShowConfirmationAsync(
            "Usu� narz�dzie", "Czy aby na pewno chcesz usun�� to narz�dzie?");
        if (confirm)
            try
            {
                await _toolService.RemoveToolAsync(tool);
                await _alertService.ShowSuccessMessageAsync(
                    "Narz�dzie zosta�o usuni�te.");
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception)
            {
                await _alertService.ShowErrorMessageAsync(
                    "Nie uda�o si� usun�� narz�dzia.");
            }
    }

    private async Task NavigateToAddToolAsync()
    {
        await Shell.Current.GoToAsync("///AddToolPage");
    }

    private async Task NavigateToDetailsAsync(Tool tool)
    {
        if (tool == null)
            return;

        await Shell.Current.GoToAsync($"///ToolsDetailsPage?ToolId={tool.Id}");
    }
}
