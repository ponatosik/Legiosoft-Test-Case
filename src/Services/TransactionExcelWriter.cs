using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using IronXL;
using IronXL.Styles;
using IronSoftware.Drawing;


namespace Legiosoft_test_case.Services;

public class TransactionExcelWriter : IExcelWriter<Transaction>
{
	public Task WriteStream(IEnumerable<Transaction> data, ref Stream stream)
	{
		WorkBook workbook = WriteWorkbook(data);
		stream = workbook.ToStream();
		return Task.CompletedTask;
	}

	public Task<byte[]> WriteBytes(IEnumerable<Transaction> data)
	{
		WorkBook workbook = WriteWorkbook(data);
		return Task.FromResult(workbook.ToByteArray());
	}

	private WorkBook WriteWorkbook(IEnumerable<Transaction> data)
	{
		WorkBook workbook = WorkBook.Create(ExcelFileFormat.XLSX);
		WorkSheet sheet = workbook.CreateWorkSheet("transactions");

		WriteHeader(sheet);

		int rowId = 2;
		foreach (var item in data)
		{
			WriteRow(sheet, item, rowId++);
		}
		AutoSizeColumns(sheet);

		return workbook;
	}

	private void WriteHeader(WorkSheet sheet)
	{
		sheet["A1"].Value = "Id";
		sheet["B1"].Value = "Name";
		sheet["C1"].Value = "Email";
		sheet["D1"].Value = "Amount";
		sheet["E1:F1"].Value = "Location";
		sheet["G1"].Value = "UtcTime";

		sheet.Merge("E1:F1");

		var headerRange = sheet["A1:G1"];

		SetAllBordes(headerRange, BorderType.Medium);
		headerRange.Style.SetBackgroundColor(Color.LightGray);
		sheet["A1:G1"].Style.HorizontalAlignment = HorizontalAlignment.Center;
	}

	private void WriteRow(WorkSheet sheet, Transaction transaction, int rowId)
	{
		sheet[$"A{rowId}"].Value = transaction.Id;
		sheet[$"B{rowId}"].Value = transaction.Name;
		sheet[$"C{rowId}"].Value = transaction.Email;
		sheet[$"D{rowId}"].DecimalValue = transaction.Amount;
		sheet[$"E{rowId}"].DecimalValue = transaction.ClientLocation.Latitude;
		sheet[$"F{rowId}"].DecimalValue = transaction.ClientLocation.Longitude;
		sheet[$"G{rowId}"].DateTimeValue = transaction.UtcTime;

		var range = sheet[$"A{rowId}:G{rowId}"];
		SetAllBordes(range, BorderType.Thin);
	}

	private void SetAllBordes(IronXL.Range range, BorderType borderType)
	{
		range.Style.LeftBorder.Type = borderType;
		range.Style.TopBorder.Type = borderType;
		range.Style.RightBorder.Type = borderType;
		range.Style.BottomBorder.Type = borderType;
	}

	private void AutoSizeColumns(WorkSheet sheet)
	{
		sheet.AutoSizeColumn(0);
		sheet.AutoSizeColumn(1);
		sheet.AutoSizeColumn(2);
		sheet.AutoSizeColumn(3);
		sheet.AutoSizeColumn(4);
		sheet.AutoSizeColumn(5);
		sheet.AutoSizeColumn(6);
	}
}
