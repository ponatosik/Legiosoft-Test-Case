using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using Spire.Xls;

namespace Legiosoft_test_case.Services;

public class TransactionExcelWriter : IExcelWriter<Transaction>
{
	public Task WriteStream(IEnumerable<Transaction> data, ref Stream stream)
	{
		Workbook workbook = WriteWorkbook(data);
		workbook.SaveToStream(stream, FileFormat.Xlsb2010);
		return Task.CompletedTask;
	}

	public Task<byte[]> WriteBytes(IEnumerable<Transaction> data)
	{
		Workbook workbook = WriteWorkbook(data);
		using MemoryStream stream = new MemoryStream();
		workbook.SaveToStream(stream);
		byte[] outData = stream.ToArray();
		return Task.FromResult(outData);
	}

	private Workbook WriteWorkbook(IEnumerable<Transaction> data)
	{
		Workbook workbook = new Workbook();
		Worksheet worksheet = workbook.Worksheets[0];

		WriteHeader(worksheet);

		int rowId = 2;
		foreach (var item in data)
		{
			WriteRow(worksheet, item, rowId++);
		}

		return workbook;
	}

	private void WriteHeader(Worksheet sheet)
	{
		sheet.Range["A1"].Text = "Id";
		sheet.Range["B1"].Text = "Name";
		sheet.Range["C1"].Text = "Email";
		sheet.Range["D1"].Text = "Amount";
		sheet.Range["E1:F1"].Text = "Location";
		sheet.Range["G1"].Text = "UtcTime";

		sheet.Range["E1:F1"].Merge();
		sheet.Range["A1:G1"].BorderInside();
		sheet.Range["A1:G1"].BorderAround();
		sheet.Range["A1:G1"].Style.HorizontalAlignment = HorizontalAlignType.Center;
	}

	private void WriteRow(Worksheet sheet, Transaction transaction, int rowId) 
	{
		sheet.Range[$"A{rowId}"].Text = transaction.Id;
		sheet.Range[$"B{rowId}"].Text = transaction.Name;
		sheet.Range[$"C{rowId}"].Text = transaction.Email;
		sheet.Range[$"D{rowId}"].NumberValue = (double)transaction.Amount;
		sheet.Range[$"E{rowId}"].NumberValue = (double)transaction.ClientLocation.Latitude;
		sheet.Range[$"F{rowId}"].NumberValue = (double)transaction.ClientLocation.Longitude;
		sheet.Range[$"G{rowId}"].DateTimeValue = transaction.UtcTime;

		sheet.Range[$"A{rowId}:G{rowId}"].BorderInside();
		sheet.Range[$"A{rowId}:G{rowId}"].BorderAround();
	}
}
