using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using ToolsManagmentMAUI.Models;

public class ToolService : INotifyPropertyChanged
{
    private readonly string _filePath;
    private ObservableCollection<Tool> _tools;

    public event EventHandler ToolAdded;
    public event PropertyChangedEventHandler PropertyChanged;

    public ToolService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "tools.json");
        _tools = new ObservableCollection<Tool>();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadToolsAsync();
    }

    public ObservableCollection<Tool> Tools
    {
        get => _tools;
        private set
        {
            _tools = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadToolsAsync()
    {
        if (!File.Exists(_filePath))
        {
            Tools = new ObservableCollection<Tool>();
            return;
        }

        string json = await File.ReadAllTextAsync(_filePath);
        List<Tool> tools = JsonSerializer.Deserialize<List<Tool>>(json) ??
            new List<Tool>();
        Tools = new ObservableCollection<Tool>(tools);
    }

    public async Task SaveToolsAsync()
    {
        string json = JsonSerializer.Serialize(Tools);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task AddToolAsync(Tool tool)
    {
        tool.Id =
            Tools.Any() ? Tools.Max(t => t.Id) + 1 : 1; // Ensure unique Id
        Tools.Add(new Tool
        {
            Id = tool.Id, Name = tool.Name, Quantity = tool.Quantity,
            Price = tool.Price
        });
        await SaveToolsAsync();
        ToolAdded?.Invoke(this, EventArgs.Empty);
        OnPropertyChanged(nameof(Tools));
    }

    public async Task UpdateToolAsync(Tool tool)
    {
        Tool? existingTool = Tools.FirstOrDefault(t => t.Id == tool.Id);
        if (existingTool != null)
        {
            existingTool.Name = tool.Name;
            existingTool.Quantity = tool.Quantity;
            existingTool.Price = tool.Price;
            await SaveToolsAsync();
            OnPropertyChanged(nameof(Tools));
        }
    }

    public async Task RemoveToolAsync(Tool tool)
    {
        Tools.Remove(tool);
        await SaveToolsAsync();
        OnPropertyChanged(nameof(Tools));
    }

    public async Task<Tool> GetToolByIdAsync(int id)
    {
        await LoadToolsAsync();
        return Tools.FirstOrDefault(t => t.Id == id);
    }

    protected void OnPropertyChanged(
        [CallerMemberName] string propertyName = null
    )
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
    }
}
