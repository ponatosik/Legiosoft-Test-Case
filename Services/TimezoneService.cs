using GeoTimeZone;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;

namespace Legiosoft_test_case.Services;

public class TimezoneService : ITimezoneService
{
	public string GetIanaTimezone(Coordinates coordinates)
	{
		double latitude = (double)coordinates.Latitude;
		double longitude = (double)coordinates.Longitude;
		return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
	}

	public TimeZoneInfo GetTimeZone(Coordinates coordinates)
	{
		string ianaTimeZone = GetIanaTimezone(coordinates);
		return TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZone);
	}

	public TimeSpan? GetTimeZoneOffset(string timeZoneId)
	{
		if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out TimeZoneInfo timeZone))
		{
			return timeZone.BaseUtcOffset;
		}
		return null;
	}

	public TimeSpan GetTimeZoneOffset(Coordinates coordinates)
	{
		TimeZoneInfo timeZone = GetTimeZone(coordinates);
		return timeZone.BaseUtcOffset;
	}

	public DateTime GetUtcTime(Coordinates coordinates, DateTime localTime)
	{
		TimeSpan timeZoneOffset = GetTimeZoneOffset(coordinates);
		return GetUtcTime(localTime, timeZoneOffset);
	}

	public DateTime? GetUtcTime(string timeZoneId, DateTime localTime)
	{
		if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out TimeZoneInfo timeZone))
		{
			TimeSpan timeZoneOffset = timeZone.BaseUtcOffset;
			return GetUtcTime(localTime, timeZoneOffset);
		}
		return null;
	}

	public DateTime GetUtcTime(DateTime localTime, TimeSpan utcOffset)
	{
		DateTimeOffset localDateTimeOffset = new DateTimeOffset(localTime, utcOffset);
		return localDateTimeOffset.UtcDateTime;
	}
}
