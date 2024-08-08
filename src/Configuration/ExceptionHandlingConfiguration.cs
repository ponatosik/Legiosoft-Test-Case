using Legiosoft_test_case.ExceptionHandlers;

namespace Legiosoft_test_case.Configuration;

public static class ExceptionHandlingConfiguration
{
	public static IServiceCollection ConfigureExceptionHandling(this IServiceCollection services)
	{
		services.AddProblemDetails();
		services.AddExceptionHandler<FormatExceptionHandler>();
		services.AddExceptionHandler<EntityAllreadyExistExceptionHandler>();
		services.AddExceptionHandler<EntityNotFoundExceptionHandler>();

		return services;
	}
}
