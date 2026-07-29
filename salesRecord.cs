namespace Pharmacy.Models;

public class SaleRecord
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalPrice { get; set; }
    public string SaleDate { get; set; } = string.Empty;
}