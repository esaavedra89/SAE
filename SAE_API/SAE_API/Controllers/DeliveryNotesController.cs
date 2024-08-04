using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAE_API.Models;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SAE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryNotesController : ControllerBase
    {
        private readonly SAEContext _context;

        public DeliveryNotesController(SAEContext context)
        {
            _context = context;
        }

        // GET: api/<DeliveryNotesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryNote>>> Get()
        {

            //List< DeliveryNote > list = null;
            //try
            //{
                if (_context.DeliveryNotes == null)
                    return NotFound();

                return await _context.DeliveryNotes.Where(x => x.Active).ToListAsync();
                //return await _context.DeliveryNotes.ToListAsync();
                //list = await _context.DeliveryNotes.ToListAsync();
                
            //}
            //catch (Exception exc)
            //{

                
            //}
            //return list;
        }

        // GET api/<DeliveryNotesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryNote>> Get(int id)
        {
            if (_context.DeliveryNotes is null)
                return NotFound();

            DeliveryNote deliveryNote = _context.DeliveryNotes.Where(m => m.Id == id && m.Active).FirstOrDefault();
            //DeliveryNote deliveryNote = _context.DeliveryNotes.Where(m => m.Id == id).FirstOrDefault();
            if (deliveryNote == null)
                return NotFound();

            List<ItemDeliveryNote> listItems = _context.ItemDeliveryNotes.Where(m => m.DeliveryNoteId == deliveryNote.Id && m.Active).ToList();
            if (listItems.Count > 0)
                deliveryNote.Items = listItems;

            return deliveryNote;
        }

        // POST api/<DeliveryNotesController>
        [HttpPost]
        public async Task<ActionResult<DeliveryNote>> Post([FromBody] DeliveryNote deliveryNote)
        {
            try
            {
                deliveryNote.CreatedDate = DateTime.Now;

                var list = await _context.DeliveryNotes.ToListAsync();

                deliveryNote.Number = list.Count + 1;

                _context.DeliveryNotes.Add(deliveryNote);

                await _context.SaveChangesAsync();

                if (deliveryNote.Id > 0)
                {
                    foreach (ItemDeliveryNote item in deliveryNote.Items)
                    {
                        item.DeliveryNoteId = deliveryNote.Id;
                        item.CreatedDate = DateTime.Now;
                        item.CreatedBy = deliveryNote.CreatedBy;

                        _context.ItemDeliveryNotes.Add(item);
                    }

                    await _context.SaveChangesAsync();
                }

                foreach (ItemDeliveryNote itemDelivery in deliveryNote.Items)
                {
                    Item item = await _context.Items.FirstOrDefaultAsync(m => m.Id == itemDelivery.ItemId);
                    if (item != null)
                    {
                        item.Quantity = item.Quantity - itemDelivery.ItemQuantity;
                        if (item.Quantity >= 0)
                        {
                            _context.Items.Update(item);
                        }
                    } 
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception exc)
            {

             
            }

            return CreatedAtAction(nameof(Get), new { id = deliveryNote.Id }, deliveryNote);
        }

        // PUT api/<DeliveryNotesController>/5
        [HttpPut]
        public async Task<DeliveryNote> Put([FromBody] DeliveryNote deliveryNote)
        {
            try
            {
                deliveryNote.UpdateddDate = DateTime.Now;

                foreach (ItemDeliveryNote itemD in deliveryNote.Items)
                {
                    if (itemD.Id == 0)
                    {
                        itemD.DeliveryNoteId = deliveryNote.Id;
                        itemD.CreatedDate = DateTime.Now;
                        itemD.CreatedBy = deliveryNote.CreatedBy;

                        _context.ItemDeliveryNotes.Add(itemD);
                    }
                    else
                    {
                        ItemDeliveryNote itemStored = await _context.ItemDeliveryNotes.Where(m => m.Id == itemD.Id && m.DeliveryNoteId == deliveryNote.Id).FirstOrDefaultAsync();
                        if (itemStored != null)
                        {
                            if (itemD.Active)
                            {
                                itemStored.ItemQuantity = itemD.ItemQuantity;
                                itemStored.PriceItem = itemD.PriceItem;
                                itemStored.TotalItem = itemD.TotalItem;
                                itemStored.Comments = itemD.Comments;
                                itemStored.ItemId = itemD.ItemId;
                                itemStored.UpdateddDate = DateTime.Now;
                                itemStored.UpdateddBy = deliveryNote.UpdateddBy;

                                _context.ItemDeliveryNotes.Update(itemStored);
                            }
                            else
                                _context.ItemDeliveryNotes.Remove(itemStored);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _context.DeliveryNotes.Update(deliveryNote);

                await _context.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return deliveryNote;
        }

        // DELETE api/<DeliveryNotesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            DeliveryNote deliveryNote =_context.DeliveryNotes.Where(m => m.Id == id).FirstOrDefault();
            if (deliveryNote != null)
            {
                List<ItemDeliveryNote> listToRemove = _context.ItemDeliveryNotes.Where(x => x.DeliveryNoteId == id).ToList();
                if (listToRemove.Count > 0)
                {
                    for (int i = 0; i < listToRemove.Count; i++)
                    {
                        ItemDeliveryNote itemToDelete = listToRemove[i];
                        itemToDelete.Active = false;

                        _context.ItemDeliveryNotes.Update(itemToDelete);
                    }

                    await _context.SaveChangesAsync();
                }

                deliveryNote.Active = false;

                _context.DeliveryNotes.Update(deliveryNote);

                await _context.SaveChangesAsync();

                return Ok();
            }
            else
                return NotFound();
        }
    }
}
