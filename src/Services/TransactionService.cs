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
	private readonly ITransactionFactory _transactionFactory;

	public TransactionService(IDbConnectionFactory connectionFactory, ITransactionFactory transactionFactory)
	{
		_connectionFactory = connectionFactory;
		_transactionFactory = transactionFactory;
	}

	public async Task AddAsync(IEnumerable<TransactionDTO> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		connection.Open();
		using IDbTransaction dbTransaction = connection.BeginTransaction();
		
		string? alreadyExisting = await FindAnyExisting(transactions, connection, dbTransaction);
		if (alreadyExisting is not null)
		{
			throw new EntityAlreadyExistException<Transaction>(alreadyExisting);
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
		
		var commandData = transactions.Select(FlattenedFromDto).ToList();
		await connection.ExecuteAsync(sql, commandData, dbTransaction);
		dbTransaction.Commit();
	}

	public async Task<IEnumerable<Transaction>> GetAllAsync()
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		
		const string sql =
			"""
			SELECT 
				t.Id, t.Name, t.Email, t.Amount, t.ClientLocation_Latitude, t.ClientLocation_Longitude,
				t.IanaTimeZoneId, t.UtcTime, t.LocalTime
			FROM Transactions t
			""";
		
		return (await connection.QueryAsync<FlattenedTransaction>(sql))
			.Select(flattened => flattened.ToTransaction());
	}

	public async Task<IEnumerable<Transaction>> GetInDateRangeAsync(DateTime from, DateTime to)
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


		var queryData = new { FromDate = from, ToDate = to };
		
		return (await connection.QueryAsync<FlattenedTransaction>(sql, queryData))
			.Select(flattened => flattened.ToTransaction());
	}

	public async Task<IEnumerable<Transaction>> GetInTransactionLocalDateRangeAsync(DateTime from, DateTime to)
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

		var queryData = new { FromDate = from, ToDate = to };
		
		
		return (await connection.QueryAsync<FlattenedTransaction>(sql, queryData)).Select(flattened => flattened.ToTransaction());
	}

	public async Task AddOrUpdateAsync(IEnumerable<TransactionDTO> transactions)
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

		var commandData = transactions.Select(FlattenedFromDto).ToList();
		
		connection.Open();
		using IDbTransaction dbTransaction = connection.BeginTransaction();
		
		await connection.ExecuteAsync(sql, commandData, transaction: dbTransaction);
		dbTransaction.Commit();
	}

	public async Task UpdateAsync(IEnumerable<TransactionDTO> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		connection.Open();
		using IDbTransaction dbTransaction = connection.BeginTransaction();
		
		string? notFound = await FindAnyNotExisting(transactions, connection, dbTransaction);
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

		var commandData = transactions.Select(FlattenedFromDto).ToList();
		await connection.ExecuteAsync(sql, commandData, dbTransaction);
		dbTransaction.Commit();
	}

	public Task<string?> FindAnyExistingAsync(IEnumerable<TransactionDTO> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		using IDbTransaction dbTransaction = connection.BeginTransaction();
		return FindAnyExisting(transactions, connection, dbTransaction);
	}

	public Task<string?> FindAnyNotExistingAsync(IEnumerable<TransactionDTO> transactions)
	{
		using IDbConnection connection = _connectionFactory.CreateConnection();
		using IDbTransaction dbTransaction = connection.BeginTransaction();
		return FindAnyNotExisting(transactions, connection, dbTransaction);
	}

	private async Task<string?> FindAnyExisting(
		IEnumerable<TransactionDTO> transactions,
		IDbConnection connection,
		IDbTransaction dbTransaction) 
	{
		var transactionIds = transactions.Select(t => t.Id).ToList();

		const string sql = 
			"""
			SELECT Id 
			FROM Transactions 
			WHERE Id IN @TransactionIds 
			LIMIT 1
			""";

		string? existingId = await connection.QuerySingleOrDefaultAsync<string>(
			sql,
			new { TransactionIds = transactionIds },
			dbTransaction);
		return existingId;
	}

  private async Task<string?> FindAnyNotExisting(
	    IEnumerable<TransactionDTO> transactions,
	    IDbConnection connection,
	    IDbTransaction dbTransaction)
    {
        const string sql =
			"""
			SELECT COUNT(1) 
			FROM
			Transactions WHERE Id = @Id;
			""";
  
		foreach (var transaction in transactions)
		{
			int count = await connection.ExecuteScalarAsync<int>(sql, new { transaction.Id }, dbTransaction);
			if (count == 0)
			{
				return transaction.Id;
			}
		}
  
		return null;
    }

	private FlattenedTransaction FlattenedFromDto(TransactionDTO transaction)
	{
		return FlattenedTransaction.From(_transactionFactory.CreateFromLocalTime(
			transaction.Id,
			transaction.Name,
			transaction.Email,
			transaction.Amount,
			transaction.ClientLocation,
			transaction.LocalTime));		
	}
}
