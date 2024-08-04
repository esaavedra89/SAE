using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAE_API.Models;

namespace SAE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemDeliveryNoteController : ControllerBase
    {
        private readonly SAEContext _context;

        public ItemDeliveryNoteController(SAEContext context)
        {
            _context = context;
        }

        // GET: api/<ItemDeliveryNoteController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDeliveryNote>>> Get()
        {
            if (_context.ItemDeliveryNotes == null)
                return NotFound();

            return await _context.ItemDeliveryNotes.ToListAsync();
        }

        // GET api/<ItemDeliveryNoteController>/5
        [HttpGet("{deliverynoteid}")]
        public async Task<ActionResult<IEnumerable<ItemDeliveryNote>>> Get(int deliverynoteid)
        {
            if (_context.ItemDeliveryNotes is null)
                return NotFound();

            List<ItemDeliveryNote> items = await _context.ItemDeliveryNotes.Where(m => m.DeliveryNoteId == deliverynoteid).ToListAsync();

            for (int i = 0; i < items.Count; i++)
            {
                ItemDeliveryNote itemNote = items[i];
                Item item = await _context.Items.Where(m => m.Id == itemNote.ItemId).FirstOrDefaultAsync();
                if (item != null)
                    itemNote.Item = item;
            }

            return items;
        }
    }
}
