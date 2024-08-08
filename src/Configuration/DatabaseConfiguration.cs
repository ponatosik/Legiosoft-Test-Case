using Legiosoft_test_case.Configuration;
using Legiosoft_test_case.Data;
using Microsoft.EntityFrameworkCore;

namespace Legiosoft_test_case.Configuration;

public static class DatabaseConfiguration
{
	public static IServiceCollection ConfigureSqlite(this IServiceCollection services, string dataSource)
	{
		var connectionString = $"Data source={dataSource}";
		var dbConnectionFactory = new SqliteConnectionFactory(connectionString);

		services.AddSingleton<IDbConnectionFactory>(dbConnectionFactory);
		services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));

		return services;
	}
}
