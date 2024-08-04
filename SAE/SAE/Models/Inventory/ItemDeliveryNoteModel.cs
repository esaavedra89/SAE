
namespace SAE.Models.Inventory
{
    public class ItemDeliveryNoteModel : EntityModel
    {
        public int ItemDeliveryNoteId { get; set; }
        public int ItemQuantity { get; set; }
        public decimal PriceItem { get; set; }
        public decimal TotalItem { get; set; }
        public string? Comments { get; set; }

        public int ItemId { get; set; }

        public int DeliveryNoteId { get; set; }
        public bool Active { get; set; }

        public ItemModel Item { get; set; }
    }
}
