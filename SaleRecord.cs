namespace Pharmacy.Models;

public class SaleRecord
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string SalesmanCode { get; set; } = string.Empty;
    
    // Batch and Medicine Details
    public int BatchId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    
    // Line Item Calculations
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double SubTotal { get; set; }
    public string SaleDate { get; set; } = string.Empty;
}