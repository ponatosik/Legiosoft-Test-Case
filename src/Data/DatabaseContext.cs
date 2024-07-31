using Legiosoft_test_case.Models;
using Microsoft.EntityFrameworkCore;

namespace Legiosoft_test_case.Data;

public class DatabaseContext : DbContext
{
	public DbSet<Transaction> Transactions { get; set; }

	public DatabaseContext(DbContextOptions options) : base(options) { } 

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<Transaction>().OwnsOne(t => t.ClientLocation);
    }
}
