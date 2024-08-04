using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAE_API.Models;

namespace SAE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtorController : ControllerBase
    {
        private readonly SAEContext _context;

        public DebtorController(SAEContext context)
        {
            _context = context;
        }

        // GET: api/<DebtorController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Debtor>>> Get()
        {
            if (_context.Debtors == null)
                return NotFound();

            return await _context.Debtors.Where(m => m.Debt > 0).ToListAsync();
        }

        // GET api/<DebtorController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Debtor>> Get(int id)
        {
            if (_context.Debtors is null)
                return NotFound();

            Debtor debtor = _context.Debtors.Where(m => m.Id == id).FirstOrDefault();
            if (debtor == null)
                return NotFound();

            List<DebtorPayment> payments = _context.DebtorPayments.Where(m => m.DebtorId == debtor.Id).ToList();
            if (payments.Count > 0)
                debtor.Payments = payments;

            return debtor;
        }

        // POST api/<DebtorController>
        [HttpPost]
        public async Task<ActionResult<Debtor>> Post([FromBody] Debtor debtor)
        {
            try
            {
                debtor.CreatedDate = DateTime.Now;

                _context.Debtors.Add(debtor);

                await _context.SaveChangesAsync();

                if (debtor.Id > 0 && debtor.Payments.Count > 0)
                {
                    foreach (DebtorPayment item in debtor.Payments)
                    {
                        item.DebtorId = debtor.Id;
                        item.CreatedDate = DateTime.Now;
                        item.CreatedBy = debtor.CreatedBy;

                        _context.DebtorPayments.Add(item);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exc)
            {


            }

            return CreatedAtAction(nameof(Get), new { id = debtor.Id }, debtor);
        }

        //PUT api/<DebtorController>/5
        [HttpPut]
        public async Task<Debtor> Put([FromBody] Debtor debtor)
        {
            try
            {
                debtor.UpdateddDate = DateTime.Now;

                _context.Debtors.Update(debtor);

                await _context.SaveChangesAsync();

                if (debtor.Id > 0 && debtor.Payments.Count > 0)
                {
                    foreach (DebtorPayment pay in debtor.Payments)
                    {
                        if (pay.Id == 0)
                        {
                            pay.DebtorId = debtor.Id;
                            pay.CreatedDate = DateTime.Now;
                            pay.CreatedBy = debtor.CreatedBy;

                            _context.DebtorPayments.Add(pay);
                        }
                        else
                        {
                            //DebtorPayment payment = _context.DebtorPayments.Where(m => m.Id == pay.Id).FirstOrDefault();
                            //if (payment != null)
                            //{
                                pay.UpdateddDate = DateTime.Now;
                                pay.UpdateddBy = debtor.UpdateddBy;

                                _context.DebtorPayments.Update(pay);
                            //}
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exc)
            {


            }

            return debtor;
        }

        // DELETE api/<DebtorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Debtor Debtor =_context.Debtors.Where(m => m.Id == id).FirstOrDefault();
            if (Debtor != null)
            {
                /* Remove payments. */
                List<DebtorPayment> payments = _context.DebtorPayments.Where(m => m.DebtorId == id).ToList();
                if (payments.Count > 0)
                {
                    foreach (DebtorPayment payment in payments)
                        _context.DebtorPayments.Remove(payment);

                    await _context.SaveChangesAsync();
                }
                /* Remove debtor. */
                _context.Debtors.Remove(Debtor);

                await _context.SaveChangesAsync();

                return Ok();
            }
            else
                return NotFound();
        }
    }
}
