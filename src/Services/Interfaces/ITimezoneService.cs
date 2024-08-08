using Legiosoft_test_case.Models;

namespace Legiosoft_test_case.Services.Interfaces;

public interface ITimezoneService
{
	string GetIanaTimezone(Coordinates coordinates);
	TimeZoneInfo GetTimeZone(Coordinates coordinates);
	TimeSpan GetTimeZoneOffset(Coordinates coordinates, DateTime localTime);
	TimeSpan? GetTimeZoneOffset(string timeZoneId, DateTime localTime);
	DateTime GetUtcTime(Coordinates coordinates, DateTime localTime);
	DateTime? GetUtcTime(string timeZoneId, DateTime localTime);
	DateTime GetUtcTime(DateTime localTime, TimeSpan utcOffset);
}
