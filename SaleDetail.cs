namespace Pharmacy.Models;

public class SaleDetail
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int BatchId { get; set; }
    
    // Historical record column in DB
    public string MedicineName { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double SubTotal => Quantity * UnitPrice;

    // Transient display properties for Cart / UI before DB save
    public string MedicineCode { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
}