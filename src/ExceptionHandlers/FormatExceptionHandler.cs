using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Legiosoft_test_case.ExceptionHandlers;

public class FormatExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
		if(exception is not FormatException)
		{
			return false;
		}
		var formatException = (FormatException)exception;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid request format.",
			Detail = GetErrorMessage(formatException),
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

	private string GetErrorMessage(FormatException exception) 
	{
		if(exception.InnerException is FormatException innerException)
		{
			var innerMessage = GetErrorMessage(innerException);
			return $"{exception.Message} {innerMessage}";
		}
		return exception.Message;
	}
}
