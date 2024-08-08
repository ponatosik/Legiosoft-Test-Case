using Dapper;
using Legiosoft_test_case.Data;
using Legiosoft_test_case.Data.Exceptions;
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

	public async Task Add(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();

		string? allreadyExisting = await FindAnyExisting(transactions, connection);
		if (allreadyExisting is not null)
		{
			throw new EntityAllreadyExistException<Transaction>(allreadyExisting!);
		}

		const string sql =
			"""
			INSERT INTO Transactions 
				(Id, Name, Email, Amount, ClientLocation_Latitude, ClientLocation_Longitude, 
				IanaTimeZoneId, UtcTime, LocalTime)
			VALUES
				(@Id, @Name, @Email, @Amount, @ClientLocation_Latitude, @ClientLocation_Longitude,
				@IanaTimeZoneId, @UtcTime, @LocalTime)
			""";

		var commandData = transactions.Select(transaction => FlattenedTransaction.From(transaction)).ToList();
		await connection.ExecuteAsync(sql, commandData);
	}

	public async Task<IEnumerable<Transaction>> GetAll()
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			SELECT 
				t.Id, t.Name, t.Email, t.Amount, t.ClientLocation_Latitude, t.ClientLocation_Longitude,
				t.IanaTimeZoneId, t.UtcTime, t.LocalTime
			FROM Transactions t
			""";

		return (await connection.QueryAsync<FlattenedTransaction>(sql)).Select(flattened => flattened.ToTransaction());
	}

	public Task<IEnumerable<Transaction>> GetInDateRange(DateTime from, DateTime to)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			SELECT 
				t.Id, t.Name, t.Email, t.Amount, t.ClientLocation_Latitude, t.ClientLocation_Longitude,
				t.IanaTimeZoneId, t.UtcTime, t.LocalTime
			FROM Transactions t
			WHERE t.UtcTime BETWEEN @FromDate AND @ToDate
			""";

		return connection.QueryAsync<Transaction>(sql, new { FromDate = from, ToDate = to });
	}

	public Task<IEnumerable<Transaction>> GetInTransactionLocalDateRange(DateTime from, DateTime to)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			SELECT 
				t.Id, t.Name, t.Email, t.Amount, t.ClientLocation_Latitude, t.ClientLocation_Longitude,
				t.IanaTimeZoneId, t.UtcTime, t.LocalTime
			FROM Transactions t
			WHERE t.LocalTime BETWEEN @FromDate AND @ToDate
			""";

		return connection.QueryAsync<Transaction>(sql, new { FromDate = from, ToDate = to });
	}

	public Task Save(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		const string sql =
			"""
			REPLACE INTO Transactions 
				(Id, Name, Email, Amount, ClientLocation_Latitude, ClientLocation_Longitude, 
				IanaTimeZoneId, UtcTime, LocalTime)
			VALUES
				(@Id, @Name, @Email, @Amount, @ClientLocation_Latitude, @ClientLocation_Longitude,
				@IanaTimeZoneId, @UtcTime, @LocalTime)
			""";

		var commandData = transactions.Select(transaction => FlattenedTransaction.From(transaction)).ToList();
		return connection.ExecuteAsync(sql, commandData);
	}

	public async Task Update(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();

		string? notFound = await FindAnyNotExisting(transactions, connection);
		if (notFound is not null)
		{
			throw new EntityNotFoundException<Transaction>(notFound!);
		}

		const string sql =
			"""
			UPDATE Transactions
			SET
				(Id, Name, Email, Amount, ClientLocation_Latitude, ClientLocation_Longitude, 
				IanaTimeZoneId, UtcTime, LocalTime)
				=
				(@Id, @Name, @Email, @Amount, @ClientLocation_Latitude, @ClientLocation_Longitude,
				@IanaTimeZoneId, @UtcTime, @LocalTime)
			WHERE Id = @Id
			""";

		var commandData = transactions.Select(transaction => FlattenedTransaction.From(transaction)).ToList();
		await connection.ExecuteAsync(sql, commandData);
	}

	public Task<string?> FindAnyExisting(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		return FindAnyExisting(transactions, connection);
	}

	public Task<string?> FindAnyNotExisting(IEnumerable<Transaction> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		return FindAnyNotExisting(transactions, connection);
	}

	private async Task<string?> FindAnyExisting(IEnumerable<Transaction> transactions, IDbConnection connection) 
	{
		var transactionIds = transactions.Select(t => t.Id).ToList();

		const string sql = 
			"""
			SELECT Id 
			FROM Transactions 
			WHERE Id IN @TransactionIds 
			LIMIT 1
			""";

		string? existingId = await connection.QuerySingleOrDefaultAsync<string>(sql, new { TransactionIds = transactionIds });
		return existingId;
	}

    private async Task<string?> FindAnyNotExisting(IEnumerable<Transaction> transactions, IDbConnection connection)
    {
        const string sql =
			"""
			SELECT COUNT(1) 
			FROM
			Transactions WHERE Id = @Id;
			""";

		foreach (var transaction in transactions)
		{
			int count = await connection.ExecuteScalarAsync<int>(sql, new { transaction.Id });
			if (count == 0)
			{
				return transaction.Id;
			}
		}

		return null;
    }
}
