using Microsoft.Data.Sqlite;
using System.Data;

namespace Legiosoft_test_case.Data;

public class SqliteConnectionFactory : IDbConnectionFactory
{
	private readonly string _connectionString;

	public SqliteConnectionFactory(string connectionString)
	{
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new SqliteConnection(_connectionString);
	}
}
