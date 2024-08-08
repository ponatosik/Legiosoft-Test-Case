using Legiosoft_test_case.Data.Exceptions;
using Legiosoft_test_case.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Legiosoft_test_case.ExceptionHandlers;

public class EntityNotFoundExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		if (exception is not EntityNotFoundException<Transaction>)
		{
			return false;
		}

		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status404NotFound,
			Title = "Transaction was not found.",
			Detail = exception.Message
		};

		httpContext.Response.StatusCode = problemDetails.Status.Value;
		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true;
	}
}
