using CsvHelper;
using Legiosoft_test_case.Configuration.Options;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Legiosoft_test_case.Services;

public class TransactionCsvReaderService : ICsvReader<Transaction>
{
	private readonly ITransactionFactory _transactionFactory;
	private readonly TransactionCsvImportOptions _options;

	public TransactionCsvReaderService(ITransactionFactory transactionFactory, IOptions<TransactionCsvImportOptions> options)
	{
		_transactionFactory = transactionFactory;
		this._options = options.Value;
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

		if (header is null)
		{
			throw new FormatException($"Failed to read cvs file. File must contain header.");
		}

		AssertHeaderContainsName(header, _options.IdFieldName);
		AssertHeaderContainsName(header, _options.NameFieldName);
		AssertHeaderContainsName(header, _options.EmailFieldName);
		AssertHeaderContainsName(header, _options.AmountFieldName);
		AssertHeaderContainsName(header, _options.ClientLocationFieldName);
		AssertHeaderContainsName(header, _options.LocalTimeFieldName);
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
			string id = csv.GetField<string>(_options.IdFieldName)!;
			string name = csv.GetField<string>(_options.NameFieldName)!;
			string email = csv.GetField<string>(_options.EmailFieldName)!;

			string amountStr = csv.GetField<string>(_options.AmountFieldName)!;
			decimal amount = ParseAmount(amountStr);

			string localTimeStr = csv.GetField<string>(_options.LocalTimeFieldName)!;
			DateTime localTime = DateTime.Parse(localTimeStr, CultureInfo.InvariantCulture);

			string clientLocationStr = csv.GetField<string>(_options.ClientLocationFieldName)!;
			Coordinates clientLocation = Coordinates.Parse(clientLocationStr);

			return _transactionFactory.CreateFromLocalTime(id, name, email, amount, clientLocation, localTime);
		}
		catch (FormatException ex)
		{
			var currentIndex = csv.CurrentIndex + 1;
			throw new FormatException($"Failed to read transaction from cvs file. Error at line {currentLine}, column {currentIndex}.", ex);
		}
	}

	private decimal ParseAmount(string amountStr)
	{
		if (!_options.AmountFieldIncludesCurrency)
		{
			return decimal.Parse(amountStr, CultureInfo.InvariantCulture);
		}
		foreach (var currency in _options.AmmountFieldPossibleCurrencySigns)
		{
			amountStr = amountStr.Replace(currency, string.Empty);
		}

		return decimal.Parse(amountStr, CultureInfo.InvariantCulture);
	}
}
