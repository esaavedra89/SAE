using SAE.Models.Inventory;
using SAE.Models.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.Models
{
    public class RootClassModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string ExceptionMessage { get; set; }

        public List<ItemModel> Items { get; set; }
        public List<DeliveryNoteModel> DeliveryNotes { get; set; }
    }
}
