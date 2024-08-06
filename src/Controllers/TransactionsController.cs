using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Legiosoft_test_case.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : Controller
{
	private readonly ITimezoneService _timezoneService;
	private readonly ITransactionService _transactionService;
	private readonly ICsvReader<Transaction> _transactionCsvReader;
	private readonly IExcelWriter<Transaction> _transactionExcleWriter;

	const string EXCEL_MIME_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

	public TransactionsController(
		ITransactionService transactionService,
		ICsvReader<Transaction> transactionCsvReader,
		IExcelWriter<Transaction> transactionExcleWriter,
		ITimezoneService timezoneService)
	{
		_transactionService = transactionService;
		_transactionCsvReader = transactionCsvReader;
		_transactionExcleWriter = transactionExcleWriter;
		_timezoneService = timezoneService;
	}

	[HttpGet]
	public async Task<IActionResult> Get(
		[FromQuery(Name = "from")] DateTime? fromDate = null,
		[FromQuery(Name = "to")] DateTime? toDate = null,
		[FromQuery(Name = "ianaZone")] string? ianaTimeZoneId = null,
		[FromQuery(Name = "transactionLocal")] bool useTransactionLocalTime = false)
	{
		if (fromDate is null && toDate is null)
		{
			var transactions = await GetAllTransactions();
			return Ok(transactions);
		}
		if (useTransactionLocalTime)
		{
			var transactions = await GetTransactionInLocalDateRange(fromDate, toDate);
			return Ok(transactions);
		}
		else
		{
			var transactions = await GetTransactionsInDateRange(fromDate, toDate, ianaTimeZoneId);
			return Ok(transactions);
		}
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] IEnumerable<Transaction> transactions)
	{
		await _transactionService.Add(transactions);
		return Created();
	}

	[HttpPut]
	public async Task<IActionResult> Put([FromBody] IEnumerable<Transaction> transactions)
	{
		await _transactionService.Save(transactions);
		return Ok();
	}

	[HttpPost("upload")]
	public async Task<IActionResult> Upload(IFormFile file)
	{
		using var csvStream = file.OpenReadStream();
		var transactions = _transactionCsvReader.Read(csvStream);
		await _transactionService.Save(transactions);
		return Created();
	}

	[HttpGet("download")]
	public async Task<IActionResult> Download(
		[FromQuery(Name = "from")] DateTime? fromDate = null,
		[FromQuery(Name = "to")] DateTime? toDate = null,
		[FromQuery(Name = "ianaZone")] string? ianaTimeZoneId = null,
		[FromQuery(Name = "transactionLocal")] bool useTransactionLocalTime = false)
	{
		if (fromDate is null && toDate is null)
		{
			var transactions = await GetAllTransactions();
			return await TransactionsFile(transactions);
		}
		if (useTransactionLocalTime)
		{
			var transactions = await GetTransactionInLocalDateRange(fromDate, toDate);
			return await TransactionsFile(transactions);
		}
		else
		{
			var transactions = await GetTransactionsInDateRange(fromDate, toDate, ianaTimeZoneId);
			return await TransactionsFile(transactions);
		}
	}

	private async Task<IEnumerable<Transaction>> GetAllTransactions()
	{
		return await _transactionService.GetAll();
	}

	private async Task<IEnumerable<Transaction>> GetTransactionInLocalDateRange(DateTime? fromDate, DateTime? toDate)
	{
		var from = fromDate ?? DateTime.MinValue;
		var to = toDate ?? DateTime.MinValue;
		return await _transactionService.GetInTransactionLocalDateRange(from, to);
	}

	private async Task<IEnumerable<Transaction>> GetTransactionsInDateRange(
		DateTime? fromDate,
		DateTime? toDate,
		string? ianaTimeZoneId)
	{
		var timeZoneId = ianaTimeZoneId ?? "Etc/GMT";
		TimeSpan timeZoneOffset = _timezoneService.GetTimeZoneOffset(timeZoneId) ?? TimeSpan.Zero;
		var from = fromDate?.Add(timeZoneOffset) ?? DateTime.MinValue;
		var to = toDate?.Add(timeZoneOffset) ?? DateTime.MaxValue;
		return await _transactionService.GetInDateRange(from, to);
	}

	private async Task<FileContentResult> TransactionsFile(IEnumerable<Transaction> transactions)
	{
		var fileBytes = await _transactionExcleWriter.WriteBytes(transactions);
		var file = new FileContentResult(fileBytes, EXCEL_MIME_TYPE);
		var timeStamp = DateTime.UtcNow.ToFileTime();
		file.FileDownloadName = $"transactions_{timeStamp}.xlsx";
		return file;
	}
}
