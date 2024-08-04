using System.ComponentModel.DataAnnotations.Schema;

namespace SAE_API.Models
{
    public class ItemDeliveryNote
    {
        public int Id { get; set; }
        public required int ItemQuantity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PriceItem { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalItem { get; set; }
        public string? Comments { get; set; }
        public required int ItemId { get; set; }
        public required int DeliveryNoteId { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }
        public required bool Active { get; set; }

        [NotMapped]
        public virtual Item Item { get; set; }

        [NotMapped]
        public virtual bool IsDelete { get; set; }
    }
}
