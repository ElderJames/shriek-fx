using System.Data.Common;
using Microsoft.Extensions.Options;

namespace Shriek.Extensions.SmartSql
{
	public class SmartSqlOptions : IOptions<SmartSqlOptions>
	{
		public SmartSqlOptions Value => this;

		public string SqlMapperPath { get; set; }

		public DbProviderFactory DbProviderFactory { get; set; }

		public string ConnectionString { get; set; }

		public string LoggingName { get; set; }

		public string ParameterPrefix { get; set; } = "@";

		public bool UseManifestResource { get; set; }
	}
}