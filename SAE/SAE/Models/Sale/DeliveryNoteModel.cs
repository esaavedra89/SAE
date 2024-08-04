using SAE.Models.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SAE.Models.Sale
{
    public class DeliveryNoteModel : EntityModel
    {
        public string CustomerName { get; set; }
        public string CustomerIdentification { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Total { get; set; }
        public string Observation { get; set; }
        public bool Deliver { get; set; }
        public bool Active { get; set; }
        public int? CustomerId { get; set; }

        public CustomerModel? Customer { get; set; }
        public List<ItemDeliveryNoteModel> Items { get; set; }


        [JsonIgnore]
        [NotMapped]
        public string BackgroundColor
        {
            get 
            {
                if (this.Deliver)
                {
                    return "#053b14";
                    
                }
                else
                {
                    return "#512BD4";
                }
            }
         }
    }
}
