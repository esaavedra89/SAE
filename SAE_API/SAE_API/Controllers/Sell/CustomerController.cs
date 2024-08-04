using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAE_API.Models;
using SAE_API.Models.Sell;

namespace SAE_API.Controllers.Sell
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly SAEContext _context;

        public CustomerController(SAEContext context)
        {
            _context = context;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            if (_context.Customers == null)
                return NotFound();

            return await _context.Customers.ToListAsync();
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(int id)
        {
            if (_context.Customers is null)
                return NotFound();

            Customer Customer = _context.Customers.Where(m => m.Id == id).FirstOrDefault();
            if (Customer == null)
                return NotFound();

            return Customer;
        }

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<ActionResult<Customer>> Post([FromBody] Customer Customer)
        {

            Customer.CreatedDate = DateTime.Now;

            _context.Customers.Add(Customer);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = Customer.Id }, Customer);
        }

        //PUT api/<CustomerController>/5
        [HttpPut]
        public async Task<Customer> Put([FromBody] Customer Customer)
        {

            Customer.UpdateddDate = DateTime.Now;

            _context.Customers.Update(Customer);

            await _context.SaveChangesAsync();

            return Customer;
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            Customer Customer =_context.Customers.Where(m => m.Id == id).FirstOrDefault();
            if (Customer != null)
            {
                _context.Customers.Remove(Customer);

                await _context.SaveChangesAsync();
            }
        }
    }
}
