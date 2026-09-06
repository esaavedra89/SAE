using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAE_API.Models;
using System.Collections.Generic;
using System.Data;

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

            // Calculamos la fecha de hace exactamente 2 meses a partir de hoy
            DateTime fechaHaceDosMeses = DateTime.Now.AddMonths(-2);

            // Filtramos donde Active sea true Y la fecha sea mayor o igual a la calculada
            return await _context.DeliveryNotes
                .Where(x => x.Active && x.Date >= fechaHaceDosMeses)
                .ToListAsync();
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
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable); // Ensure atomic stock update.
            try
            {
                if (deliveryNote == null || deliveryNote.Items == null || !deliveryNote.Items.Any())
                    return BadRequest("La nota debe tener items.");

                if (deliveryNote.Items.Any(x => x.ItemId <= 0 || x.ItemQuantity <= 0))
                    return BadRequest("ItemId y ItemQuantity deben ser validos.");

                if (!deliveryNote.Items.Any(x => x.Active && x.ItemId > 0 && x.ItemQuantity > 0))
                    return BadRequest("La nota debe tener al menos un item activo.");

                deliveryNote.Date = DateTime.SpecifyKind(deliveryNote.Date.Date, DateTimeKind.Unspecified);
                deliveryNote.CreatedDate = DateTime.Now;

                var list = await _context.DeliveryNotes.AsNoTracking().ToListAsync();

                deliveryNote.Number = list.Count + 1;

                _context.DeliveryNotes.Add(deliveryNote);

                await _context.SaveChangesAsync();

                if (deliveryNote.Id > 0)
                {
                    foreach (ItemDeliveryNote item in deliveryNote.Items.Where(x => x.Active))
                    {
                        item.DeliveryNoteId = deliveryNote.Id;
                        item.CreatedDate = DateTime.Now;
                        item.CreatedBy = deliveryNote.CreatedBy;

                        _context.ItemDeliveryNotes.Add(item);
                    }

                    await _context.SaveChangesAsync();
                }

                var grouped = deliveryNote.Items
                    .Where(x => x.Active)
                    .GroupBy(x => x.ItemId)
                    .Select(g => new { ItemId = g.Key, Qty = g.Sum(x => x.ItemQuantity) })
                    .ToList(); // Consolidate quantity per item.

                foreach (var row in grouped)
                {
                    Item item = await _context.Items.FirstOrDefaultAsync(m => m.Id == row.ItemId);
                    if (item == null)
                        return BadRequest($"Item {row.ItemId} no existe.");

                    if (item.Quantity < row.Qty)
                        return BadRequest($"Stock insuficiente para item {item.Id}.");

                    item.Quantity -= row.Qty;
                    _context.Items.Update(item);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync(); // Avoid partial writes.
                throw;
            }

            return CreatedAtAction(nameof(Get), new { id = deliveryNote.Id }, deliveryNote);
        }

        // PUT api/<DeliveryNotesController>/5
        [HttpPut]
        public async Task<ActionResult<DeliveryNote>> Put([FromBody] DeliveryNote deliveryNote)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable); // Keep note and stock in sync.
            try
            {
                if (deliveryNote == null || deliveryNote.Id <= 0)
                    return BadRequest("Nota invalida.");

                if (deliveryNote.Items == null)
                    deliveryNote.Items = new List<ItemDeliveryNote>();

                deliveryNote.UpdateddDate = DateTime.Now;

                DeliveryNote storedNote = await _context.DeliveryNotes.FirstOrDefaultAsync(x => x.Id == deliveryNote.Id && x.Active);
                if (storedNote == null)
                    return NotFound();

                List<ItemDeliveryNote> storedLines = await _context.ItemDeliveryNotes
                    .Where(x => x.DeliveryNoteId == deliveryNote.Id && x.Active)
                    .ToListAsync();

                var incomingByItem = deliveryNote.Items
                    .Where(x => x.Active && x.ItemQuantity > 0)
                    .GroupBy(x => x.ItemId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.ItemQuantity));

                var storedByItem = storedLines
                    .GroupBy(x => x.ItemId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.ItemQuantity));

                var allItemIds = incomingByItem.Keys.Union(storedByItem.Keys).Distinct().ToList();
                foreach (int itemId in allItemIds)
                {
                    incomingByItem.TryGetValue(itemId, out int newQty);
                    storedByItem.TryGetValue(itemId, out int oldQty);
                    int delta = newQty - oldQty; // Positive consumes stock, negative returns stock.
                    if (delta == 0) continue;

                    Item stockItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == itemId);
                    if (stockItem == null)
                        return BadRequest($"Item {itemId} no existe.");

                    if (delta > 0)
                    {
                        if (stockItem.Quantity < delta)
                            return BadRequest($"Stock insuficiente para item {stockItem.Id}.");

                        stockItem.Quantity -= delta;
                    }
                    else
                    {
                        stockItem.Quantity += Math.Abs(delta);
                    }

                    _context.Items.Update(stockItem);
                }

                foreach (ItemDeliveryNote itemD in deliveryNote.Items)
                {
                    if (itemD.Id == 0)
                    {
                        if (!itemD.Active || itemD.ItemQuantity <= 0 || itemD.ItemId <= 0)
                            continue;

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
                            itemStored.ItemQuantity = itemD.ItemQuantity;
                            itemStored.PriceItem = itemD.PriceItem;
                            itemStored.TotalItem = itemD.TotalItem;
                            itemStored.Comments = itemD.Comments;
                            itemStored.ItemId = itemD.ItemId;
                            itemStored.Active = itemD.Active;
                            itemStored.UpdateddDate = DateTime.Now;
                            itemStored.UpdateddBy = deliveryNote.UpdateddBy;

                            _context.ItemDeliveryNotes.Update(itemStored);
                        }
                    }
                }

                HashSet<int> incomingIds = deliveryNote.Items.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet();
                foreach (ItemDeliveryNote oldLine in storedLines.Where(x => !incomingIds.Contains(x.Id)))
                {
                    oldLine.Active = false; // Soft-delete removed lines.
                    oldLine.UpdateddDate = DateTime.Now;
                    oldLine.UpdateddBy = deliveryNote.UpdateddBy;
                    _context.ItemDeliveryNotes.Update(oldLine);
                }

                await _context.SaveChangesAsync();

                storedNote.CustomerName = deliveryNote.CustomerName;
                storedNote.CustomerIdentification = deliveryNote.CustomerIdentification;
                storedNote.Date = DateTime.SpecifyKind(deliveryNote.Date.Date, DateTimeKind.Unspecified);
                storedNote.PaymentMethod = deliveryNote.PaymentMethod;
                storedNote.Subtotal = deliveryNote.Subtotal;
                storedNote.Discount = deliveryNote.Discount;
                storedNote.DiscountPercentage = deliveryNote.DiscountPercentage;
                storedNote.Total = deliveryNote.Total;
                storedNote.Observation = deliveryNote.Observation;
                storedNote.Deliver = deliveryNote.Deliver;
                storedNote.UpdateddDate = DateTime.Now;
                storedNote.UpdateddBy = deliveryNote.UpdateddBy;

                _context.DeliveryNotes.Update(storedNote);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return Ok(deliveryNote);
        }

        // DELETE api/<DeliveryNotesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable); // Keep rollback and delete together.
            DeliveryNote deliveryNote = _context.DeliveryNotes.Where(m => m.Id == id && m.Active).FirstOrDefault();
            if (deliveryNote != null)
            {
                List<ItemDeliveryNote> listToRemove = _context.ItemDeliveryNotes.Where(x => x.DeliveryNoteId == id && x.Active).ToList();
                if (listToRemove.Count > 0)
                {
                    var grouped = listToRemove
                        .GroupBy(x => x.ItemId)
                        .Select(g => new { ItemId = g.Key, Qty = g.Sum(x => x.ItemQuantity) })
                        .ToList(); // Sum quantities by item.

                    foreach (var row in grouped)
                    {
                        Item item = await _context.Items.FirstOrDefaultAsync(x => x.Id == row.ItemId);
                        if (item != null)
                        {
                            item.Quantity += row.Qty;
                            _context.Items.Update(item);
                        }
                    }

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
                await tx.CommitAsync();

                return Ok();
            }
            else
                return NotFound();
        }
    }
}
