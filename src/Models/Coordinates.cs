
using System.Globalization;

namespace Legiosoft_test_case.Models;

public class Coordinates
{
	public decimal Latitude { get; set; }
	public decimal Longitude { get; set; }

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

		return new Coordinates
		{
			Latitude = latitude,
			Longitude = longitude
		};
	}
}
