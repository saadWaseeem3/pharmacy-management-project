namespace Pharmacy.Models
{
    public class MedicineWithBatch
    {
        public int MedicineId { get; set; }
        public string MedicineCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public int TotalQuantity { get; set; }
    }
}