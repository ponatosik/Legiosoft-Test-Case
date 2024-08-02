using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Legiosoft_test_case.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : Controller
{
	private readonly ITransactionService _transactionService;
	private readonly ICsvReader<Transaction> _transactionCsvReader;
	private readonly IExcelWriter<Transaction> _transactionExcleWriter;

	const string EXCEL_MIME_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

	public TransactionsController(
		ITransactionService transactionService,
		ICsvReader<Transaction> transactionCsvReader,
		IExcelWriter<Transaction> transactionExcleWriter)
	{
		_transactionService = transactionService;
		_transactionCsvReader = transactionCsvReader;
		_transactionExcleWriter = transactionExcleWriter;
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		return Ok(await _transactionService.GetAll());
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
	public async Task<IActionResult> Download()
	{
		var transactions = await _transactionService.GetAll();
		var fileBytes = await _transactionExcleWriter.WriteBytes(transactions);
		var file = new FileContentResult(fileBytes, EXCEL_MIME_TYPE);

		var timeStamp = DateTime.UtcNow.ToFileTime();
		file.FileDownloadName = $"transactions_{timeStamp}.xlsx";
		return file;
	}
}
