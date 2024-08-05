using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;

namespace Legiosoft_test_case.Services;

public class TransactionFactory : ITransactionFactory
{
	private readonly ITimezoneService _timezoneService;

	public TransactionFactory(ITimezoneService timezoneService)
	{
		_timezoneService = timezoneService;
	}

	public Transaction Create(
		string id,
		string name,
		string email,
		decimal amount,
		Coordinates clientLocation,
		DateTime utcTime)
	{
		return new Transaction()
		{
			Id = id,
			Name = name,
			Email = email,
			Amount = amount,
			ClientLocation = clientLocation,
			UtcTime = utcTime
		};
	}

	public Transaction CreateFromLocalTime(string id, string name, string email, decimal amount, Coordinates clientLocation, DateTime localTime)
	{
		DateTime utcTime = _timezoneService.GetUtcTime(clientLocation, localTime);
		return Create(id, name, email, amount, clientLocation, utcTime);
	}
}
