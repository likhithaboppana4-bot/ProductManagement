namespace LibraryManagement.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string SKU { get; set; } // unique

        public string Category { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        public string Manufacturer { get; set; }
        public double Weight { get; set; }
        public string Dimensions { get; set; } // e.g. "10x5x2 cm"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
