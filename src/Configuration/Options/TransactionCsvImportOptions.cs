namespace Legiosoft_test_case.Configuration.Options;

public class TransactionCsvImportOptions
{
	public string IdFieldName { get; init; } = "transaction_id";
	public string NameFieldName { get; init; } = "name";
	public string EmailFieldName { get; init; } = "email";
	public string AmountFieldName { get; init; } = "amount";
	public string ClientLocationFieldName { get; init; } = "client_location";
	public string LocalTimeFieldName { get; init; }  = "transaction_date";

	public bool AmountFieldIncludesCurrency { get; init; } = true;
	public List<string> AmountFieldPossibleCurrencySigns { get; init; } = ["$"];
}
