namespace Pharmacy.Models;

    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Medicine Null";
        public string Category { get; set; } = "Category Null";
        public double Price { get; set; }
        public int Quantity { get; set; } = 0;
    }

