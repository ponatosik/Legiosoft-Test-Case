namespace Legiosoft_test_case.Configuration.Options;

public class TransactionExcelExportOptions
{
	public string IdHeaderText { get; set; } = "Transaction";
	public string NameHeaderText { get; set; } = "Name";
	public string EmailHeaderText { get; set; } = "Email";
	public string AmountHeaderText { get; set; } = "Amount";
	public string ClientLocationHeaderText { get; set; } = "Location";
	public string UtcTimeHeaderText { get; set; }  = "UTC time";

	public string WorksheetName { get; set; } = "Transactions";
	public bool AutoSizeColumns { get; set; } = true;
}
