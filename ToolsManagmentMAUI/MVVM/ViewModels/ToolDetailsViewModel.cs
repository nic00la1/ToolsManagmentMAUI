using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace ToolsManagmentMAUI.ViewModels;

[QueryProperty(nameof(ToolId), nameof(ToolId))]
public class ToolDetailsViewModel : BaseViewModel
{
    private readonly ToolService _toolService;
    private readonly AlertService _alertService;
    private Tool _tool;
    private int _toolId;

    public Tool Tool
    {
        get => _tool;
        set => SetProperty(ref _tool, value);
    }

    public int ToolId
    {
        get => _toolId;
        set
        {
            _toolId = value;
            LoadToolId(value);
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public ToolDetailsViewModel()
    {
        _toolService = new ToolService();
        _alertService = new AlertService();
        SaveCommand = new Command(OnSave);
        DeleteCommand = new Command(OnDelete);
    }

    public async void LoadToolId(int toolId)
    {
        try
        {
            Tool = await _toolService.GetToolByIdAsync(toolId);
        }
        catch (Exception)
        {
            await _alertService.ShowErrorMessageAsync(
                "Nie uda³o siê za³adowaæ narzêdzia.");
        }
    }

    private async void OnSave()
    {
        try
        {
            await _toolService.UpdateToolAsync(Tool);
            await _alertService.ShowSuccessMessageAsync(
                "Dane zosta³y zapisane poprawnie.");
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception)
        {
            await _alertService.ShowErrorMessageAsync(
                "Nie uda³o siê zapisaæ danych.");
        }
    }

    private async void OnDelete()
    {
        bool confirm = await _alertService.ShowConfirmationAsync(
            "Usuñ narzêdzie", "Czy aby na pewno chcesz usun¹æ to narzêdzie?");
        if (confirm)
            try
            {
                await _toolService.RemoveToolAsync(Tool);
                await _alertService.ShowSuccessMessageAsync(
                    "Narzêdzie zosta³o usuniête.");
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception)
            {
                await _alertService.ShowErrorMessageAsync(
                    "Nie uda³o siê usun¹æ narzêdzia.");
            }
    }
}
