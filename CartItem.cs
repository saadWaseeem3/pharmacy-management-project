namespace Pharmacy.Models;

public class CartItem
{
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalPrice => Quantity * UnitPrice;
}