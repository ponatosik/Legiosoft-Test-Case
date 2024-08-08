using GeoTimeZone;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using TimeZoneConverter;

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
		return TZConvert.GetTimeZoneInfo(ianaTimeZone);
	}

	public TimeSpan? GetTimeZoneOffset(string timeZoneId, DateTime localTime)
	{
		if (TZConvert.TryGetTimeZoneInfo(timeZoneId, out TimeZoneInfo? timeZone))
		{
			return timeZone.GetUtcOffset(localTime);
		}
		return null;
	}

	public TimeSpan GetTimeZoneOffset(Coordinates coordinates, DateTime localTime)
	{
		TimeZoneInfo timeZone = GetTimeZone(coordinates);
		return timeZone.GetUtcOffset(localTime);
	}

	public DateTime GetUtcTime(Coordinates coordinates, DateTime localTime)
	{
		TimeSpan timeZoneOffset = GetTimeZoneOffset(coordinates, localTime);
		return GetUtcTime(localTime, timeZoneOffset);
	}

	public DateTime? GetUtcTime(string timeZoneId, DateTime localTime)
	{
		if (TZConvert.TryGetTimeZoneInfo(timeZoneId, out TimeZoneInfo timeZone))
		{
			TimeSpan timeZoneOffset = timeZone.BaseUtcOffset;
			return GetUtcTime(localTime, timeZoneOffset);
		}
		return null;
	}

	public DateTime GetUtcTime(DateTime localTime, TimeSpan utcOffset)
	{
		return localTime.Subtract(utcOffset);
	}
}
