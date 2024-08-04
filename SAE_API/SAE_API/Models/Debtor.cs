using System.ComponentModel.DataAnnotations.Schema;

namespace SAE_API.Models
{
    public class Debtor
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public string? CustomerIdentification { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalDebt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Debt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Summation { get; set; }
        public string? Observation { get; set; }
        public required string RelatedDeliveryNote { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }

        [NotMapped]
        public List<DebtorPayment> Payments { get; set; }
    }
}
