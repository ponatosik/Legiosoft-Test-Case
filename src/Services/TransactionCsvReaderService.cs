using CsvHelper;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using System.Globalization;

namespace Legiosoft_test_case.Services;

public class TransactionCsvReaderService : ICsvReader<Transaction>
{
	private readonly ITransactionFactory _transactionFactory;

	public TransactionCsvReaderService(ITransactionFactory transactionFactory)
	{
		_transactionFactory = transactionFactory;
	}

	public IEnumerable<Transaction> Read(Stream stream)
	{
		using StreamReader streamReader = new StreamReader(stream);
		using CsvReader csv = new(streamReader, CultureInfo.InvariantCulture);

		VerifyHeader(csv);

		int lineNumber = 1;
		LinkedList<Transaction> transactions = new LinkedList<Transaction>();
		while (csv.Read())
		{
			Transaction transaction = ReadTransaction(csv, lineNumber++);
			transactions.AddLast(transaction);
		}

		return transactions;
	}

	private void VerifyHeader(CsvReader csv)
	{
		csv.Read();
		csv.ReadHeader();

		string[]? header = csv.HeaderRecord;

		if(header is null)
		{
			throw new FormatException($"Failed to read cvs file. File must contain header.");
		}

		AssertHeaderContainsName(header, "transaction_id");
		AssertHeaderContainsName(header, "name");
		AssertHeaderContainsName(header, "email");
		AssertHeaderContainsName(header, "amount");
		AssertHeaderContainsName(header, "transaction_date");
		AssertHeaderContainsName(header, "client_location");
	}

	private void AssertHeaderContainsName(string[] header, string headerName)
	{
		if (!header.Contains(headerName))
		{
			throw new FormatException($"Failed to read transaction from cvs file. File must contain {headerName} column.");
		}
	}

	private Transaction ReadTransaction(CsvReader csv, int currentLine)
	{
		try
		{
			string id = csv.GetField<string>("transaction_id")!;
			string name = csv.GetField<string>("name")!;
			string email = csv.GetField<string>("email")!;

			string amountStr = csv.GetField<string>("amount")!;
			decimal amount = decimal.Parse(amountStr.Replace("$", ""), CultureInfo.InvariantCulture);

			string localTimeStr = csv.GetField<string>("transaction_date")!;
			DateTime localTime = DateTime.Parse(localTimeStr, CultureInfo.InvariantCulture);

			string clientLocationStr = csv.GetField<string>("client_location")!;
			Coordinates clientLocation = Coordinates.Parse(clientLocationStr);

			return _transactionFactory.CreateFromLocalTime(id, name, email, amount, clientLocation, localTime);
		}
		catch (FormatException ex)
		{
			var currentIndex = csv.CurrentIndex + 1;
			throw new FormatException($"Failed to read transaction from cvs file. Error at line {currentLine}, column {currentIndex}.", ex);
		}
	}
}
