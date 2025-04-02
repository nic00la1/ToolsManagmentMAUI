using ToolsManagmentMAUI.Models;

public class ShoppingCartItem
{
    public Tool Tool { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Quantity * Tool.Price;
}
