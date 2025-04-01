using System.Collections.ObjectModel;
using System.Windows.Input;
using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.Services;
using Microsoft.Maui.Controls;
using ToolsManagmentMAUI.MVVM.Views;

namespace ToolsManagmentMAUI.ViewModels
{
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
            Tools = new ObservableCollection<Tool>();
            LoadToolsCommand = new Command(async () => await LoadToolsAsync());
            DeleteToolCommand = new Command<Tool>(async (tool) => await DeleteToolAsync(tool));
            NavigateToAddToolCommand = new Command(async () => await NavigateToAddToolAsync());
            NavigateToDetailsCommand = new Command<Tool>(async (tool) => await NavigateToDetailsAsync(tool));
        }

        private async Task LoadToolsAsync()
        {
            var tools = await _toolService.LoadToolsAsync();
            Tools.Clear();
            foreach (var tool in tools)
            {
                Tools.Add(tool);
            }
        }

        private async Task DeleteToolAsync(Tool tool)
        {
            var confirm = await _alertService.ShowConfirmationAsync("Potwierdzenie", "Czy na pewno chcesz usun¹æ to narzêdzie?");
            if (confirm)
            {
                Tools.Remove(tool);
                await _toolService.SaveToolsAsync(Tools.ToList());
            }
        }

        private async Task NavigateToAddToolAsync()
        {
            await Shell.Current.GoToAsync("///AddToolPage");
        }


        private async Task NavigateToDetailsAsync(Tool tool)
        {
            var navigationParameter = new Dictionary<string, object>
    {
        { "Tool", tool }
    };
            await Shell.Current.GoToAsync("///ToolsDetailsPage", navigationParameter);
        }
    }
}