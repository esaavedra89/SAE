using System.ComponentModel.DataAnnotations.Schema;

namespace SAE_API.Models
{
    public class DeliveryNote
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerIdentification { get; set; }
        public required int Number { get; set; }
        public required DateTime Date { get; set; }
        public string? PaymentMethod { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal DiscountPercentage { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Total { get; set; }
        public string? Observation { get; set; }
        public required bool Deliver { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }
        public required bool Active { get; set; }

        [NotMapped]
        public virtual List<ItemDeliveryNote> Items { get; set; }
    }
}
