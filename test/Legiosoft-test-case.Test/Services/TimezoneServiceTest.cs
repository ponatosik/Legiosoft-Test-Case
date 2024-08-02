using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services;
using Legiosoft_test_case.Services.Interfaces;
using System.Globalization;

namespace Legiosoft_test_case.Test.Services;

public class TimezoneServiceTest
{
	ITimezoneService _timezoneService = new TimezoneService();

	[Theory]
	[InlineData(50.45, 30.52, "Europe/Kyiv")]
	[InlineData(34.05, -118.24, "America/Los_Angeles")]
	[InlineData(51.50, -0.11, "Europe/London")]
	[InlineData(34.65, 139.83, "Asia/Tokyo")]
	public void GetIanaTiezone_ValidCoordinates_ShouldReturnTimeZoneIanaId(
		 decimal latitude, decimal longitude, string expectedIanaId)
	{
		var coordinates = new Coordinates
		{
			Latitude = latitude,
			Longitude = longitude
		};

		var actual = _timezoneService.GetIanaTimezone(coordinates);

		Assert.StartsWith(expectedIanaId, actual);
	}

	[Theory]
	[InlineData(50.45, 30.52, "2:00")]
	[InlineData(34.05, -118.24, "-8:00")]
	[InlineData(51.50, -0.11, "0:00")]
	[InlineData(34.65, 139.83, "9:00")]
	public void GetTimeZoneOffser_ValidCoordinates_ShouldReturnTimeZoneOffset(
		 decimal latitude, decimal longitude, string expectedUtcOffset)
	{
		var expectedOffset = TimeSpan.Parse(expectedUtcOffset);
		var coordinates = new Coordinates
		{
			Latitude = latitude,
			Longitude = longitude
		};

		var actual = _timezoneService.GetTimeZoneOffset(coordinates);

		Assert.Equal(expectedOffset, actual);
	}

	[Theory]
	[InlineData(50.45, 30.52, "01/18/2018 09:22", "01/18/2018 07:22")]
	[InlineData(34.05, -118.24, "01/17/2018 23:22", "01/18/2018 07:22")]
	[InlineData(51.50, -0.11, "01/18/2018 07:22", "01/18/2018 07:22")]
	[InlineData(34.65, 139.83, "01/19/2018 07:22", "01/18/2018 22:22")]
	public void GetUtcTime_ValidCoordinates_ShouldReturnUtc0Time(
		 decimal latitude, decimal longitude, string localTimeStr, string expectedTimeStr)
	{
		var localTime = DateTime.Parse(localTimeStr, CultureInfo.InvariantCulture);
		var expectedTime = DateTime.Parse(expectedTimeStr, CultureInfo.InvariantCulture);
		var coordinates = new Coordinates
		{
			Latitude = latitude,
			Longitude = longitude
		};

		var actual = _timezoneService.GetUtcTime(coordinates, localTime);

		Assert.Equal(expectedTime, actual);
	}

	[Theory]
	[InlineData("01/18/2018 05:22", "2:00", "01/18/2018 03:22")]
	[InlineData("01/17/2018 23:22", "-8:00", "01/18/2018 07:22")]
	[InlineData("01/18/2018 07:22", "0:00", "01/18/2018 07:22")]
	[InlineData("01/19/2018 07:22", "9:00", "01/18/2018 22:22")]
	public void GetUtcTime_ValidTimeOffset_ShouldReturnUtc0Time(
		 string localTimeStr, string timeOffsetStr, string expectedTimeStr)
	{
		var localTime = DateTime.Parse(localTimeStr, CultureInfo.InvariantCulture);
		var expectedTime = DateTime.Parse(expectedTimeStr, CultureInfo.InvariantCulture);
		var timeOffset = TimeSpan.Parse(timeOffsetStr, CultureInfo.InvariantCulture);

		var actual = _timezoneService.GetUtcTime(localTime, timeOffset);

		Assert.Equal(expectedTime, actual);
	}
}
