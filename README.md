# Legiosoft-Test-Case

This repository contains a Web API that allows users to import transactions from CSV files and export them to Excel files.

## Features

- Import transactions from CSV files
- Export transactions to Excel files
- RESTfull API endpoints for managing transactions
- Interactive documentation (Swagger)
- Retrieve transactions in specified date range
- Endpoint for retrieving transaction for January 2024

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Entity Framework Core CLI Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/transaction-import-export-api.git
   cd transaction-import-export-api
   ```

2. Navigate to the source folder:
   ```
   cd src
   ```

3. Update the database (creates Transactions.db file):
   ```
   dotnet ef database update
   ```

4. Run the application via editor, or by running:
   ```
   dotnet run
   ```
