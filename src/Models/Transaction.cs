namespace Legiosoft_test_case.Models;

public class Transaction
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public decimal Amount { get; set; }
	public Coordinates ClientLocation { get; set; }
	public DateTime UtcTime { get; set; }
}
