namespace Pharmacy.Models;

public class Batch
{
    public int Id { get; set;}
    public int MedicineId { get; set;}
    public string BatchNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
}