using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SAE_API.Models;

namespace SAE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly SAEContext _context;

        public ItemsController(SAEContext context)
        {
            _context = context;
        }

        // GET: api/<ItemsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> Get()
        {
            if (_context.Items == null)
                return NotFound();

            return await _context.Items.ToListAsync();
        }

        // GET api/<ItemsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> Get(int id)
        {
            if (_context.Items is null)
                return NotFound();

            Item item = _context.Items.Where(m => m.Id == id).FirstOrDefault();
            if (item == null)
                return NotFound();

            return item;
        }

        // POST api/<ItemsController>
        [HttpPost]
        public async Task<ActionResult<Item>> Post([FromBody] Item item)
        {
            _context.Items.Add(item);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        // PUT api/<ItemsController>/5
        [HttpPut]
        public async Task<Item> Put([FromBody] Item item)
        {
            item.UpdateddDate = DateTime.Now;
            _context.Items.Update(item);

            await _context.SaveChangesAsync();

            return item;
        }

        // DELETE api/<ItemsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Item item =_context.Items.Where(m => m.Id == id).FirstOrDefault();
            if (item != null)
            {
                _context.Items.Remove(item);

                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
