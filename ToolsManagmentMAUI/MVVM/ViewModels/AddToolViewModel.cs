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
            Tool = new Tool();
            AddToolCommand = new Command(async () => await AddToolAsync());
        }

        private async Task AddToolAsync()
        {
            var tools = await _toolService.LoadToolsAsync();
            tools.Add(Tool);
            await SaveToolsToJsonAsync(tools);
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async Task SaveToolsToJsonAsync(List<Tool> tools)
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "tools.json");
            var json = JsonSerializer.Serialize(tools);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}