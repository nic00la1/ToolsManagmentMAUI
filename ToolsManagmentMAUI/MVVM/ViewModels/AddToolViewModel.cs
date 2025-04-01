using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.Services;
using Microsoft.Maui.Controls;

namespace ToolsManagmentMAUI.ViewModels
{
    public class AddToolViewModel : BindableObject
    {
        private readonly ToolService _toolService;
        private readonly AlertService _alertService;
        private Tool _tool;

        public Tool Tool
        {
            get => _tool;
            set
            {
                _tool = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddToolCommand { get; }

        public AddToolViewModel()
        {
            _toolService = new ToolService();
            _alertService = new AlertService();
            Tool = new Tool();
            AddToolCommand = new Command(async () => await AddToolAsync());
        }

        private async Task AddToolAsync()
        {
            await _toolService.AddToolAsync(Tool);
            await _alertService.ShowMessageAsync("Sukces", "Narz�dzie zosta�o pomy�lnie dodane.");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}