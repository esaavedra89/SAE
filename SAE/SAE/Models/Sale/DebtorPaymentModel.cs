
namespace SAE.Models.Sale
{
    public class DebtorPaymentModel : EntityModel
    {
        public decimal PaymentAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime Date { get; set; }
        public string? Observation { get; set; }
        public int DebtorId { get; set; }
    }
}
