namespace Legiosoft_test_case.Configuration.Options;

public class TransactionCsvImportOptions
{
	public string IdFieldName { get; set; } = "transaction_id";
	public string NameFieldName { get; set; } = "name";
	public string EmailFieldName { get; set; } = "email";
	public string AmountFieldName { get; set; } = "amount";
	public string ClientLocationFieldName { get; set; } = "client_location";
	public string LocalTimeFieldName { get; set; }  = "transaction_date";

	public bool AmountFieldIncludesCurrency { get; set; } = true;
	public List<string> AmmountFieldPossibleCurrencySigns { get; set; } = new() { "$" };
}
