using Legiosoft_test_case.Models;

namespace Legiosoft_test_case.Services.Interfaces;

public interface ITransactionService
{
	Task AddAsync(IEnumerable<TransactionDTO> transactions);
	Task UpdateAsync(IEnumerable<TransactionDTO> transactions);
	Task AddOrUpdateAsync(IEnumerable<TransactionDTO> transactions);
	Task<IEnumerable<Transaction>> GetAllAsync();
	Task<IEnumerable<Transaction>> GetInDateRangeAsync(DateTime utcFrom, DateTime utcTo);	
	Task<IEnumerable<Transaction>> GetInTransactionLocalDateRangeAsync(DateTime from, DateTime to);
	Task<string?> FindAnyExistingAsync(IEnumerable<TransactionDTO> transactions);
	Task<string?> FindAnyNotExistingAsync(IEnumerable<TransactionDTO> transactions);
}
