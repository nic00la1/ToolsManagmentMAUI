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
                "Nie uda�o si� za�adowa� narz�dzia.");
        }
    }

    private async void OnSave()
    {
        try
        {
            await _toolService.UpdateToolAsync(Tool);
            await _alertService.ShowSuccessMessageAsync(
                "Dane zosta�y zapisane poprawnie.");
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception)
        {
            await _alertService.ShowErrorMessageAsync(
                "Nie uda�o si� zapisa� danych.");
        }
    }

    private async void OnDelete()
    {
        bool confirm = await _alertService.ShowConfirmationAsync(
            "Usu� narz�dzie", "Czy aby na pewno chcesz usun�� to narz�dzie?");
        if (confirm)
            try
            {
                await _toolService.RemoveToolAsync(Tool);
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
}
