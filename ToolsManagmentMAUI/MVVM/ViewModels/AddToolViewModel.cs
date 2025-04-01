using System.Windows.Input;
using ToolsManagmentMAUI.Models;
using ToolsManagmentMAUI.Services;
using Microsoft.Maui.Controls;

namespace ToolsManagmentMAUI.ViewModels
{
    public class AddToolViewModel : BindableObject
    {
        private readonly ToolService _toolService;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public ICommand AddToolCommand { get; }

        public AddToolViewModel()
        {
            _toolService = new ToolService();
            AddToolCommand = new Command(async () => await AddToolAsync());
        }

        private async Task AddToolAsync()
        {
            var newTool = new Tool
            {
                Name = Name,
                Description = Description,
                Category = Category
            };

            var tools = await _toolService.LoadToolsAsync();
            tools.Add(newTool);
            await _toolService.SaveToolsAsync(tools);

            await Shell.Current.GoToAsync("..");
        }
    }
}
