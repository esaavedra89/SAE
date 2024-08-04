using System.ComponentModel.DataAnnotations.Schema;

namespace SAE_API.Models
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? BarCode { get; set; }
        public string? SellerCode { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SellerPrice { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PriceCost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ProfitPercentage { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPriceCalculated { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal FinalPrice { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }
    }
}
