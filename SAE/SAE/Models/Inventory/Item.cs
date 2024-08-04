using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.Models.Inventory
{
    public class Item : EntityModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? BarCode { get; set; }
        public string? SellerCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellerPrice { get; set; }
        public decimal PriceCost { get; set; }
        public decimal ProfitPercentage { get; set; }
        public decimal TotalPriceCalculated { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
