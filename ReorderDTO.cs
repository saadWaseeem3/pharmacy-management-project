namespace Pharmacy.Models;

public class ReorderDTO
{
    public string CompanyName { get; set; } = string.Empty;
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public int MinReorderLevel { get; set; }
    public int CurrentStock { get; set; }
    public int ReorderQuantity { get; set; }
}