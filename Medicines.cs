namespace Pharmacy.Models;

    public class Medicine
    {
        public int Id { get; set; }
        public string MedicineCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }

        //Threshold and Supplier Attributes

        public int CompanyId { get; set; } = 1;
        public int MinReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        
    }

