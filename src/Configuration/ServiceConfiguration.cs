using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services;
using Legiosoft_test_case.Services.Interfaces;

namespace Legiosoft_test_case.Configuration;

public static class ServiceConfiguration
{
	public static IServiceCollection ConfigureServices(this IServiceCollection services) 
	{
		services.AddSingleton<ITimezoneService, TimezoneService>();
		services.AddSingleton<ITransactionService, TransactionService>();
		services.AddSingleton<ICsvReader<Transaction>, TransactionCsvReaderService>();
		services.AddSingleton<IExcelWriter<Transaction>, TransactionExcelWriter>();
		services.AddSingleton<ITransactionFactory, TransactionFactory>();

		return services;
	}
}
