namespace ToolsManagmentMAUI.Models;

public class Tool
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }

    public string FirstLetter => !string.IsNullOrEmpty(Name)
        ? Name[0].ToString()
        : string.Empty;
}
