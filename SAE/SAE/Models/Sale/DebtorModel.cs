
namespace SAE.Models.Sale
{
    public class DebtorModel : EntityModel
    {
        public required string CustomerName { get; set; }
        public string? CustomerIdentification { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal Debt { get; set; }
        public decimal Summation { get; set; }
        public string? Observation { get; set; }
        public required string RelatedDeliveryNote { get; set; }
        public int? CustomerId { get; set; }

        public CustomerModel? Customer { get; set; }

        public List<DebtorPaymentModel> Payments { get; set; }
    }
}
