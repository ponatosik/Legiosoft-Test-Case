using Legiosoft_test_case.Controllers;

namespace Legiosoft_test_case.Configuration;

public static class SwaggerConfiguration
{
	public static IServiceCollection ConfigureApiDocumentation(this IServiceCollection services)
	{
		services.AddSwaggerGen(config =>
		{
			var assembly = typeof(TransactionsController).Assembly;
			var xmlFile = $"{assembly.GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			config.IncludeXmlComments(xmlPath);
		});

		return services;
	}
}
