namespace Pharmacy.Models;

    public class Medicine
    {
        public int Id { get; set; }
        public string MedicineCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; } = 0;
    }

