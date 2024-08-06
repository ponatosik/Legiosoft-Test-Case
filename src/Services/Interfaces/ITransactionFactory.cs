using Legiosoft_test_case.Models;

namespace Legiosoft_test_case.Services.Interfaces;

public interface ITransactionFactory
{
	public Transaction Create(
		string id,
		string name,
		string email,
		decimal amount,
		Coordinates clientLocation,
		string ianaTimeZoneId,
		DateTime utcTime,
		DateTime localTime);

	public Transaction CreateFromLocalTime(
		string id,
		string name,
		string email,
		decimal amount,
		Coordinates clientLocation,
		DateTime localTime);
}
