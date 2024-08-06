using Legiosoft_test_case.Data;
using Legiosoft_test_case.ExceptionHandlers;
using Legiosoft_test_case.Models;
using Legiosoft_test_case.Services;
using Legiosoft_test_case.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.UseSqlite("TestDb.db");
builder.Services.AddSingleton<ITimezoneService, TimezoneService>();
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<ICsvReader<Transaction>, TransactionCsvReaderService>();
builder.Services.AddSingleton<IExcelWriter<Transaction>, TransactionExcelWriter>();
builder.Services.AddSingleton<ITransactionFactory, TransactionFactory>();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<FormatExceptionHandler>();

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
