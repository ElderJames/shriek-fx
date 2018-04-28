using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MapperContainer = SmartSql.MapperContainer;

namespace Shriek.Extensions.SmartSql
{
	public static class ServiceCollectionExtensions
	{
		public static void UseSmartSql(this IServiceCollection services, Action<SmartSqlOptions> optionAction)
		{
			var options = new SmartSqlOptions();
			optionAction(options);

			services.AddSingleton(sp =>
			{
				var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
				return MapperContainer.Instance.GetSqlMapper(loggerFactory, string.Empty, new NativeConfigLoader(loggerFactory, options));
			});
		}
	}
}