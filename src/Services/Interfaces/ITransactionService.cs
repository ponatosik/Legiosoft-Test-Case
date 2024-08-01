using Legiosoft_test_case.Models;

namespace Legiosoft_test_case.Services.Interfaces;

public interface ITransactionService
{
	Task Add(IEnumerable<Transaction> transactions);
	Task Update(IEnumerable<Transaction> transactions);
	Task Save(IEnumerable<Transaction> transactions);
	Task<IEnumerable<Transaction>> GetAll();
	Task<IEnumerable<Transaction>> GetInDateRange(DateTime from, DateTime to);	
}
