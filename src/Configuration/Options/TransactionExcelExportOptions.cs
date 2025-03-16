namespace Legiosoft_test_case.Configuration.Options;

public class TransactionExcelExportOptions
{
	public string IdHeaderText { get; init; } = "Transaction";
	public string NameHeaderText { get; init; } = "Name";
	public string EmailHeaderText { get; init; } = "Email";
	public string AmountHeaderText { get; init; } = "Amount";
	public string ClientLocationHeaderText { get; init; } = "Location";
	public string UtcTimeHeaderText { get; init; }  = "UTC time";

	public string WorksheetName { get; init; } = "Transactions";
	public bool AutoSizeColumns { get; init; } = true;
}
