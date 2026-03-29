namespace InventarioApi.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
