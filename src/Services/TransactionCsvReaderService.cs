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

		//Todo: verify headers name

		csv.Read();
		csv.ReadHeader();

		LinkedList<Transaction> transactions = new LinkedList<Transaction>();
		while (csv.Read())
		{
			transactions.AddLast(ReadTransaction(csv));
		}

		return transactions;
	}

	private Transaction ReadTransaction(CsvReader csv)
	{
		string id = csv.GetField<string>("transaction_id")!;
		string name = csv.GetField<string>("name")!;
		string email = csv.GetField<string>("email")!;
		string amountStr = csv.GetField<string>("amount")!;
		DateTime localTime = csv.GetField<DateTime>("transaction_date");
		string clientLocationStr = csv.GetField<string>("client_location")!;

		decimal amount = decimal.Parse(amountStr.Replace("$",""), CultureInfo.InvariantCulture);
		Coordinates clientLocation = Coordinates.Parse(clientLocationStr);

		return _transactionFactory.CreateFromLocalTime(id, name, email, amount, clientLocation, localTime);
	}
}
