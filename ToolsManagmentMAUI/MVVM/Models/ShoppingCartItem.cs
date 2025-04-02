using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToolsManagmentMAUI.Models;

public class ShoppingCartItem : INotifyPropertyChanged
{
    private int _quantity;
    private decimal _totalPrice;

    public Tool Tool { get; set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SubtotalPrice));
            }
        }
    }

    public decimal TotalPrice
    {
        get => _totalPrice;
        set
        {
            if (_totalPrice != value)
            {
                _totalPrice = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal SubtotalPrice => Tool.Price * Quantity;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string propertyName = null
    )
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
    }
}
