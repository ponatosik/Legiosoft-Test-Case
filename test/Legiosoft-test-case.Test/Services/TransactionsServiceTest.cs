using Legiosoft_test_case.Data;
using Legiosoft_test_case.Data.Exceptions;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services;
using Legiosoft_test_case.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Legiosoft_test_case.Test.Services;

[Collection(nameof(DatabaseCollection))]
public class TransactionsServiceTest
{
	ITransactionService _transactionsService;
	DatabaseContext _dbContext;

	static Coordinates GREENWICH_COORDINATES => new Coordinates(51.47M, -0.000001M);
	static string GREENWICH_TIMEZONE_ID => "Etc/GMT";

	Transaction[] _persistedTransactions =
	[
		new Transaction()
		{
			Id = "id1",
			Name = "user1",
			Email = "email111@mail.com",
			Amount = 10,
			ClientLocation = GREENWICH_COORDINATES,
			IanaTimeZoneId = GREENWICH_TIMEZONE_ID,
			UtcTime = DateTime.Parse("01/01/2024"),
			LocalTime = DateTime.Parse("01/01/2024")
		},
		new Transaction()
		{
			Id = "id2",
			Name = "user2",
			Email = "email222@mail.com",
			Amount = 20,
			ClientLocation = GREENWICH_COORDINATES,
			IanaTimeZoneId = GREENWICH_TIMEZONE_ID,
			UtcTime = DateTime.Parse("02/02/2024"),
			LocalTime = DateTime.Parse("02/02/2024")
		},
		new Transaction() {
			Id = "id3",
			Name = "user3",
			Email = "email333@mail.com",
			Amount = 33,
			ClientLocation = GREENWICH_COORDINATES,
			IanaTimeZoneId = GREENWICH_TIMEZONE_ID,
			UtcTime = DateTime.Parse("03/03/2024"),
			LocalTime = DateTime.Parse("03/03/2024")
		}
	];

	public TransactionsServiceTest()
	{
		var services = new ServiceCollection();
		services.UseSqlite("file::memory:?cache=shared");
		services.AddTransient<ITransactionService, TransactionService>();

		var serviceProvider = services.BuildServiceProvider();

		_dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
		_transactionsService = serviceProvider.GetRequiredService<ITransactionService>();

		PrepareDatabase();
	}

	private void PrepareDatabase()
	{
		_dbContext.Database.EnsureCreated();
		_dbContext.Database.ExecuteSqlRaw("DELETE FROM Transactions");
		_dbContext.Transactions.AddRange(_persistedTransactions);
		_dbContext.SaveChanges();
		_dbContext.ChangeTracker.Clear();
	}

	[Fact]
	public async Task Add_ValidTransactions_ShoulPersistTransactions()
	{
		Transaction[] transactions =
		[
			new Transaction() { Id = "new_id1", Name = "n1", Email = "e1", Amount = 1, ClientLocation = GREENWICH_COORDINATES,
				IanaTimeZoneId = GREENWICH_TIMEZONE_ID, UtcTime = DateTime.UtcNow, LocalTime = DateTime.UtcNow },
			new Transaction() { Id = "new_id2", Name = "n2", Email = "e2", Amount = 2, ClientLocation = GREENWICH_COORDINATES,
				IanaTimeZoneId = GREENWICH_TIMEZONE_ID, UtcTime = DateTime.UtcNow.AddDays(2), LocalTime =DateTime.UtcNow.AddDays(2) }
		];

		await _transactionsService.Add(transactions);

		List<Transaction> persistedTransactions = _dbContext.Transactions.ToList();
		Assert.Contains(persistedTransactions, t => t.Id == transactions[0].Id);
		Assert.Contains(persistedTransactions, t => t.Id == transactions[1].Id);
	}

	[Fact]
	public async Task Add_ExistingTransactions_ShoulThrowEntityAllreadyExists()
	{
		Transaction[] transactions =
		[
			new Transaction() { Id = "new_id1", Name = "n1", Email = "e1", Amount = 1, ClientLocation = GREENWICH_COORDINATES,
				IanaTimeZoneId = GREENWICH_TIMEZONE_ID, UtcTime = DateTime.UtcNow, LocalTime = DateTime.UtcNow },
			_persistedTransactions[0]
		];

		await Assert.ThrowsAsync<EntityAllreadyExistException<Transaction>>(async () =>
			await _transactionsService.Add(transactions));
	}

	[Fact]
	public async Task Update_ExistingTransactions_ShouldPersistChanges()
	{
		Transaction[] transactionsToUpdate = [_persistedTransactions[0], _persistedTransactions[1]];

		transactionsToUpdate[0].Name = "NewName1";
		transactionsToUpdate[1].Name = "NewName2";
		await _transactionsService.Update(transactionsToUpdate);

		List<Transaction> persistedTransactions = _dbContext.Transactions.ToList();
		Assert.Contains(persistedTransactions, t => t.Id == transactionsToUpdate[0].Id && t.Name == transactionsToUpdate[0].Name);
		Assert.Contains(persistedTransactions, t => t.Id == transactionsToUpdate[1].Id && t.Name == transactionsToUpdate[1].Name);
	}

	[Fact]
	public async Task Update_NotExistingTransactions_ShouldThrowEntityNotFound()
	{
		Transaction newTransaction =
			new Transaction()
			{
				Id = "new_id1",
				Name = "n1",
				Email = "e1",
				Amount = 1,
				ClientLocation = GREENWICH_COORDINATES,
				IanaTimeZoneId = GREENWICH_TIMEZONE_ID,
				UtcTime = DateTime.UtcNow,
				LocalTime = DateTime.UtcNow
			};
		Transaction[] transactionsToUpdate = [_persistedTransactions[0], newTransaction];

		transactionsToUpdate[0].Name = "NewName1";
		await Assert.ThrowsAsync<EntityNotFoundException<Transaction>>(async () =>
			await _transactionsService.Update(transactionsToUpdate));
	}

	[Fact]
	public async Task Save_NewAndExistingTransactions_ShouldPersistChangesAndNewTransactions()
	{
		var newTransaction = new Transaction()
		{
			Id = "new_id",
			Name = "new user",
			Email = "NewEmail@mail.com",
			Amount = 999,
			ClientLocation = GREENWICH_COORDINATES,
			IanaTimeZoneId = GREENWICH_TIMEZONE_ID,
			UtcTime = DateTime.Parse("04/04/2024"),
			LocalTime = DateTime.Parse("04/04/2024")
		};
		var existingTransaction = _persistedTransactions[0];
		Transaction[] transactions = [existingTransaction, newTransaction];

		existingTransaction.Name = "NewName1";
		await _transactionsService.Save(transactions);

		List<Transaction> updatedTransactions = _dbContext.Transactions.ToList();
		Assert.Contains(updatedTransactions, t => t.Id == transactions[0].Id && t.Name == transactions[0].Name);
		Assert.Contains(updatedTransactions, t => t.Id == transactions[1].Id && t.Name == transactions[1].Name);
	}

	[Fact]
	public async Task GetAll_PersistedTransactions_ShouldReturnAllPersistedTransactions()
	{

		var result = (await _transactionsService.GetAll()).ToList();

		Assert.Equal(_persistedTransactions.Length, result.Count);
		Assert.Contains(result, t => t.Id == _persistedTransactions[0].Id && t.Name == _persistedTransactions[0].Name);
		Assert.Contains(result, t => t.Id == _persistedTransactions[1].Id && t.Name == _persistedTransactions[1].Name);
	}

	[Fact]
	public async Task GetInDateRange_PersistedTransactions_ShouldReturnPersistedTransactionsInDateRange()
	{
		var fromDate = _persistedTransactions[1].UtcTime;
		var toDate = _persistedTransactions[2].UtcTime;
		Transaction[] expectedTransactions = [_persistedTransactions[1], _persistedTransactions[2]];

		var result = (await _transactionsService.GetInDateRange(fromDate, toDate)).ToList();

		Assert.Equal(expectedTransactions.Length, result.Count);
		Assert.Contains(result, t => t.Id == expectedTransactions[0].Id && t.Name == expectedTransactions[0].Name);
		Assert.Contains(result, t => t.Id == expectedTransactions[1].Id && t.Name == expectedTransactions[1].Name);
	}
}
