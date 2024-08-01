using Microsoft.EntityFrameworkCore;

namespace Legiosoft_test_case.Data;

public static class DatabaseConfiguration
{
	public static IServiceCollection UseSqlite(this IServiceCollection services, string dataSource)
	{
		var connectionString = $"Data source={dataSource}";
		var dbConnectionFactory = new SqliteConnectionFactory(connectionString);

		services.AddSingleton<IDbConnectionFactory>(dbConnectionFactory);
		services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));

		return services;
	}
}
