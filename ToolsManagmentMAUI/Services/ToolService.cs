using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ToolsManagmentMAUI.Models;

public class ToolService
{
    private readonly string _filePath;

    public event EventHandler ToolAdded;

    public ToolService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "tools.json");
    }

    public async Task<List<Tool>> LoadToolsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Tool>();
        }

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Tool>>(json) ?? new List<Tool>();
    }

    public async Task SaveToolsAsync(List<Tool> tools)
    {
        var json = JsonSerializer.Serialize(tools);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task AddToolAsync(Tool tool)
    {
        var tools = await LoadToolsAsync();
        tools.Add(tool);
        await SaveToolsAsync(tools);
        ToolAdded?.Invoke(this, EventArgs.Empty);
    }
}