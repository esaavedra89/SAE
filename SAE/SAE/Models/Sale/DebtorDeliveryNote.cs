using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.Models.Sale
{
    public class DebtorDeliveryNote : EntityModel
    {
        public int DeliveryNoteId { get; set; }
        public int DebtorId { get; set; }

        public DeliveryNoteModel DeliveryNote { get; set; }
    }
}
