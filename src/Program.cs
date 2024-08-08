using Legiosoft_test_case.Configuration;
using Legiosoft_test_case.Configuration.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Custom configuration, look at Configuration folder
builder.Services.ConfigureApiDocumentation();
builder.Services.ConfigureServices();
builder.Services.ConfigureSqlite("Transactions.db");
builder.Services.ConfigureExceptionHandling();

var configuration = builder.Configuration;
builder.Services.Configure<TransactionCsvImportOptions>(configuration.GetSection("CsvTransactionImport"));
builder.Services.Configure<TransactionExcelExportOptions>(configuration.GetSection("ExcelTransactionExport"));

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
