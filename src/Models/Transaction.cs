namespace Legiosoft_test_case.Models;

public class Transaction
{
	public required string Id { get; init; }
	public required string Name { get; set; }
	public required string Email { get; set; }
	public required decimal Amount { get; set; }
	public required Coordinates ClientLocation { get; set; }
	public required string IanaTimeZoneId { get; set; }
	public required DateTime UtcTime { get; set; }
	public required DateTime LocalTime { get; set; }
}
