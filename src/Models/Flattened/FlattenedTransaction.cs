namespace Legiosoft_test_case.Models.Flattened;

// This class is required to map nested types (like Coordinates)
// Dapper does't suppor mapping of EF Core's "owned" types
public record FlattenedTransaction
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public decimal Ammount { get; set; }
	public decimal ClientLocation_Latitude { get; set; }
	public decimal ClientLocation_Longitude { get; set; }
	public DateTime UtcTime { get; set; }

	public static FlattenedTransaction From(Transaction transaction) =>
		new FlattenedTransaction
		{
			Id = transaction.Id,
			Name = transaction.Name,
			Email = transaction.Email,
			Ammount = transaction.Ammount,
			ClientLocation_Latitude = transaction.ClientLocation.Latitude,
			ClientLocation_Longitude = transaction.ClientLocation.Longitude,
			UtcTime = transaction.UtcTime
		};

	public Transaction ToTransaction()
	{
		return new Transaction()
		{
			Id = this.Id,
			Name = this.Name,
			Email = this.Email,
			Ammount = this.Ammount,
			ClientLocation = new Coordinates()
			{
				Latitude = this.ClientLocation_Latitude,
				Longitude = this.ClientLocation_Longitude
			},
			UtcTime = this.UtcTime
		};
	}
}
