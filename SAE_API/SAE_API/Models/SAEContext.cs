using Microsoft.EntityFrameworkCore;
using SAE_API.Models.Sell;

namespace SAE_API.Models
{
    public class SAEContext : DbContext
    {
        public SAEContext(DbContextOptions<SAEContext> options)
        : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Debtor> Debtors { get; set; } = null!;
        public DbSet<DebtorPayment> DebtorPayments { get; set; } = null!;
        public DbSet<DeliveryNote> DeliveryNotes { get; set; } = null!;
        public DbSet<ItemDeliveryNote> ItemDeliveryNotes { get; set; } = null!;

    }
}
