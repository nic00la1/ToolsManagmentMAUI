using ToolsManagmentMAUI.Models;
using Microsoft.Maui.Controls;

namespace ToolsManagmentMAUI.ViewModels
{
    [QueryProperty(nameof(Tool), "Tool")]
    public class ToolDetailsViewModel : BindableObject
    {
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
    }
}
