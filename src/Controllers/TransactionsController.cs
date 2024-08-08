using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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

	/// <summary>
	/// Retrieves a list of transactions based on specified parameters.
	/// </summary>
	/// 
	/// <remarks>
	/// If the "transactionLocal" parameter is used, the timezone will be ignored, and the transactions will be returned in the local time where they occurred.
	/// </remarks>
	/// 
	/// <param name="fromDate">The starting date to include transactions from.</param>
	/// <param name="toDate">The ending date to include transactions to.</param>
	/// <param name="ianaTimeZoneId">The time zone identifier (IANA format) for the request. If not specified, the server's time zone will be used.</param>
	/// <param name="useTransactionLocalTime">If true, the transactions will be returned in the local time where they occurred, ignoring the specified time zone.</param>
	///
	/// <returns>
	/// A list of transactions based on the specified parameters. If no date range is provided, all transactions will be returned.
	/// </returns>
	///
	/// <response code="200">The operation was successful, and the transactions are returned.</response>
	/// <response code="400">The request parameters are invalid.</response>
	/// <response code="500">An internal server error occurred while processing the request.</response>
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

	/// <summary>
	/// Creates a new set of transactions.
	/// </summary>
	///
	/// <param name="transactions">
	/// Collection of transactions to be created.
	/// UtcTime and TimezoneId would be calculated base of client location.
	/// </param>
	///
	/// <returns>
	/// A 201 Created response indicating that the transactions have been successfully created.
	/// </returns>
	///
	/// <response code="201">The transactions have been successfully created.</response>
	/// <response code="400">The request body is invalid or missing required information.</response>
	/// <response code="409">One or more of the transactions already exist in the system.</response>
	/// <response code="500">An internal server error occurred while processing the request.</response>
	[HttpPost]
	public async Task<IActionResult> Post([FromBody] IEnumerable<TransactionDTO> transactions)
	{
		await _transactionService.AddAsync(transactions);
		return Created();
	}

	/// <summary>
	/// Updates a set of existing transactions.
	/// </summary>
	///
	/// <param name="transactions">
	/// Collection of transactions to be created.
	/// UtcTime and TimezoneId would be calculated base of client location.
	/// </param>
	///
	/// <returns>
	/// A 200 OK response indicating that the transactions have been successfully updated.
	/// </returns>
	///
	/// <response code="200">The transactions have been successfully updated.</response>
	/// <response code="400">The request body is invalid or missing required information.</response>
	/// <response code="404">One or more of the transactions do not exist in the system.</response>
	/// <response code="500">An internal server error occurred while processing the request.</response>
	[HttpPut]
	public async Task<IActionResult> Put([FromBody] IEnumerable<TransactionDTO> transactions)
	{
		await _transactionService.UpdateAsync(transactions);
		return Ok();
	}

	/// <summary>
	/// Uploads a CSV file containing transactions and creates or updates them in the system.
	/// </summary>
	///
	/// <param name="file">
	/// The CSV file containing the transaction data to be uploaded.
	/// The file should be in a format that can be read and parsed.
	/// </param>
	///
	/// <returns>
	/// A 201 Created response indicating that the transactions have been successfully uploaded and processed.
	/// </returns>
	///
	/// <response code="201">The transactions have been successfully uploaded and processed.</response>
	/// <response code="400">The request does not contain a valid CSV file, or the file format is not supported.</response>
	/// <response code="409">One or more of the transactions in the file already exist in the system and could not be updated.</response>
	/// <response code="500">An internal server error occurred while processing the request or reading the CSV file.</response>
	[HttpPost("upload")]
	public async Task<IActionResult> Upload(IFormFile file)
	{
		using var csvStream = file.OpenReadStream();
		var transactions = _transactionCsvReader.Read(csvStream);
		var dtos = transactions.Select(TransactionDTO.From);
		await _transactionService.AddOrUpdateAsync(dtos);
		return Created();
	}

	/// <summary>
	/// Downloads excel file containing a list of transactions based on the specified parameters.
	/// </summary>
	///
	/// <remarks>
	/// If the "transactionLocal" parameter is used, the timezone will be ignored, and the transactions will be returned in the local time where they occurred.
	/// </remarks>
	///
	/// <param name="fromDate">The starting date to include transactions from.</param>
	/// <param name="toDate">The ending date to include transactions to.</param>
	/// <param name="ianaTimeZoneId">The time zone identifier (IANA format) for the request. If not specified, the server's time zone will be used.</param>
	/// <param name="useTransactionLocalTime">If true, the transactions will be included in the file in the local time where they occurred, ignoring the specified time zone.</param>
	///
	/// <returns>
	/// A file containing a list of transactions based on the specified parameters. If no date range is provided, all transactions will be included.
	/// </returns>
	///
	/// <response code="200">The operation was successful, and the transactions file is returned.</response>
	/// <response code="400">The request parameters are invalid.</response>
	/// <response code="500">An internal server error occurred while processing the request.</response>
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

	/// <summary>
	/// Retrieves a list of transactions for January 2024.
	/// </summary>
	///
	/// <remarks>
	/// This endpoint is a convenience method that calls the main "Get" endpoint with a predefined date range for January 2024.
	/// The same rules and behavior from the "Get" endpoint apply to this method.
	/// </remarks>
	///
	/// <param name="ianaTimeZoneId">The time zone identifier (IANA format) for the request. If not specified, the server's time zone will be used.</param>
	/// <param name="useTransactionLocalTime">If true, the transactions will be returned in the local time where they occurred, ignoring the specified time zone.</param>
	///
	/// <returns>
	/// A list of transactions for January 2024 based on the specified parameters.
	/// </returns>
	///
	/// <response code="200">The operation was successful, and the transactions are returned.</response>
	/// <response code="400">The request parameters are invalid.</response>
	/// <response code="500">An internal server error occurred while processing the request.</response>
	[HttpGet("january2024")]
	public Task<IActionResult> GetJanuary2024(
		[FromQuery(Name = "ianaZone")] string? ianaTimeZoneId = null,
		[FromQuery(Name = "transactionLocal")] bool useTransactionLocalTime = false)
	{
		var from = DateTime.Parse("01/01/2024 00:00:00", CultureInfo.InvariantCulture);
		var to = DateTime.Parse("01/02/2024 00:00:00", CultureInfo.InvariantCulture);
		return Get(from, to, ianaTimeZoneId, useTransactionLocalTime);
	}

	/// <summary>
	/// Downloads excel file containing a list of transactions for January 2024.
	/// </summary>
	///
	/// <remarks>
	/// This endpoint is a convenience method that calls the main "Download" endpoint with a predefined date range for January 2024.
	/// The same rules and behavior from the "Download" endpoint apply to this method.
	/// </remarks>
	///
	/// <param name="ianaTimeZoneId">The time zone identifier (IANA format) for the request. If not specified, the server's time zone will be used.</param>
	/// <param name="useTransactionLocalTime">If true, the transactions will be included in the file in the local time where they occurred, ignoring the specified time zone.</param>
	///
	/// <returns>
	/// A file containing a list of transactions for January 2024 based on the specified parameters.
	/// </returns>
	///
	/// <response code="200">The operation was successful, and the transactions file is returned.</response>
	/// <response code="400">The request parameters are invalid.</response>
	/// <response code="500">An internal server error occurred while processing the request.</response>
	[HttpGet("january2024/download")]
	public Task<IActionResult> DownloadJanuary2024(
		[FromQuery(Name = "ianaZone")] string? ianaTimeZoneId = null,
		[FromQuery(Name = "transactionLocal")] bool useTransactionLocalTime = false)
	{
		var from = DateTime.Parse("01/01/2024 00:00:00", CultureInfo.InvariantCulture);
		var to = DateTime.Parse("01/02/2024 00:00:00", CultureInfo.InvariantCulture);
		return Download(from, to, ianaTimeZoneId, useTransactionLocalTime);
	}

	private async Task<IEnumerable<Transaction>> GetAllTransactions()
	{
		return await _transactionService.GetAllAsync();
	}

	private async Task<IEnumerable<Transaction>> GetTransactionInLocalDateRange(DateTime? fromDate, DateTime? toDate)
	{
		var from = fromDate ?? DateTime.MinValue;
		var to = toDate ?? DateTime.MinValue;
		return await _transactionService.GetInTransactionLocalDateRangeAsync(from, to);
	}

	private async Task<IEnumerable<Transaction>> GetTransactionsInDateRange(
		DateTime? fromDate,
		DateTime? toDate,
		string? ianaTimeZoneId)
	{
		var timeZoneId = ianaTimeZoneId ?? "Etc/GMT";
		TimeSpan timeZoneOffset = _timezoneService.GetTimeZoneOffset(timeZoneId, DateTime.UtcNow) ?? TimeSpan.Zero;
		var from = fromDate?.Add(timeZoneOffset) ?? DateTime.MinValue;
		var to = toDate?.Add(timeZoneOffset) ?? DateTime.MaxValue;
		return await _transactionService.GetInDateRangeAsync(from, to);
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
