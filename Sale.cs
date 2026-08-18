namespace Pharmacy.Models;

public class Sale
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int SalesmanId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.Now;
    public double GrandTotal { get; set; }

    // Navigation property for relational mapping
    public List<SaleDetail> Items { get; set; } = new();
}