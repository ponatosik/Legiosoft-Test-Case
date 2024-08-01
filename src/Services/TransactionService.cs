using Dapper;
using Legiosoft_test_case.Data;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Models.Flattened;
using Legiosoft_test_case.Services.Interfaces;
using System.Data;

namespace Legiosoft_test_case.Services;

public class TransactionService : ITransactionService
{
	private readonly IDbConnectionFactory _connectionFactory;


	public TransactionService(IDbConnectionFactory connectionFactory)
	{
		_connectionFactory = connectionFactory;
	}

	public Task Add(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			INSERT INTO Transactions 
			(Id, Name, Email, Amount, ClientLocation_Latitude, ClientLocation_Longitude, UtcTime)
			VALUES
			(@Id, @Name, @Email, @Amount, @ClientLocation_Latitude, @ClientLocation_Longitude, @UtcTime)
			""";

		var commandData = transactions.Select(transaction => FlattenedTransaction.From(transaction)).ToList();
		return connection.ExecuteAsync(sql, commandData);
	}

	public async Task<IEnumerable<Transaction>> GetAll()
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			SELECT t.Id, t.Name, t.Email, t.Amount, t.ClientLocation_Latitude, t.ClientLocation_Longitude, t.UtcTime
			FROM Transactions t
			""";

		return (await connection.QueryAsync<FlattenedTransaction>(sql)).Select(flattened => flattened.ToTransaction());
	}

	public Task<IEnumerable<Transaction>> GetInDateRange(DateTime from, DateTime to)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			SELECT t.id, t.Name, t.Email, t.Amount, t.ClientLocation_Latitute, t.ClientLocation_Longitude, t.UtcTime
			FROM Transactions t
			WHER t.UtcTime BETWEEN @FromDate AND @ToDate
			""";

		return connection.QueryAsync<Transaction>(sql, new { FromDate = from, ToDate = to });
	}

	public Task Save(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			REPLACE INTO Transactions 
			(Id, Name, Email, Amount, ClientLocation_Latitude, ClientLocation_Longitude, UtcTime)
			VALUES
			(@Id, @Name, @Email, @Amount, @ClientLocation_Latitude, @ClientLocation_Longitude, @UtcTime)
			""";

		var commandData = transactions.Select(transaction => FlattenedTransaction.From(transaction)).ToList();
		return connection.ExecuteAsync(sql, commandData);
	}

	public Task Update(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			UPDATE Transactions t
			SET Name = @Name,
			SET Email = @Email,
			SET Amount = @Amount,
			SET ClientLocation_Latitude = @ClientLocation_Latitude,
			SET CLientLocation_Longitude = @CLientLocation_Longitude,
			SET UtcTime = @UtcTime
			WHERE t.Id == @Id
			""";

		var commandData = transactions.Select(transaction => FlattenedTransaction.From(transaction)).ToList();
		return connection.ExecuteAsync(sql, commandData);
	}
}
