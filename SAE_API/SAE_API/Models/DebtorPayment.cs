using System.ComponentModel.DataAnnotations.Schema;

namespace SAE_API.Models
{
    public class DebtorPayment
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PaymentAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime Date { get; set; }
        public string? Observation { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }
        public required int DebtorId { get; set; }
    }
}
