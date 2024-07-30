using Microsoft.EntityFrameworkCore;

namespace Legiosoft_test_case.Data;

public static class DatabaseConfiguration
{
	public static IServiceCollection UseSqlite(this IServiceCollection services, string dataSource)
	{
		var connectionString = $"Data source={dataSource}";
		var dbConnectionFactory = new SqliteConnectionFactory(dataSource);


// SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
		services.AddSingleton<IDbConnectionFactory>(dbConnectionFactory);
		services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));

		return services;
	}
}
