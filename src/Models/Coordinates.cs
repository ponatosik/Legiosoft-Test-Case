using System.Globalization;

namespace Legiosoft_test_case.Models;

public class Coordinates
{
	public decimal Latitude { get; init; }
	public decimal Longitude { get; init; }

	public Coordinates() { }
	public Coordinates(decimal latitude, decimal longitude)
	{
		Latitude = latitude;
		Longitude = longitude;
	}

	public static Coordinates Parse(string str)
	{
		if (string.IsNullOrEmpty(str) || !str.Contains(','))
		{
			throw new FormatException("Invalid coordinates format. Input string must be in the format 'latitude, longitude'.");
		}
		var parts = str.Split(',');

		if (!decimal.TryParse(parts[0].Trim(), CultureInfo.InvariantCulture, out decimal latitude))
		{
			throw new FormatException("Invalid coordinates format. Input string must be in the format 'latitude, longitude'.");
		}
		if (!decimal.TryParse(parts[1].Trim(), CultureInfo.InvariantCulture, out decimal longitude))
		{
			throw new FormatException("Invalid coordinates format. Input string must be in the format 'latitude, longitude'.");
		}

		if(latitude is < -180 or > 180)
		{
			throw new FormatException("Invalid coordinates format. latitude must be in range from -180 to 180.");
		}
		if (longitude is < -180 or > 180)
		{
			throw new FormatException("Invalid coordinates format. longitude must be in range from -180 to 180.");
		}

		return new Coordinates
		{
			Latitude = latitude,
			Longitude = longitude
		};
	}
}
