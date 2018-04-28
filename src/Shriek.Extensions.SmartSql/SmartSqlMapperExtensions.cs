using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSql.Abstractions;

namespace Shriek.Extensions.SmartSql
{
	public static class SmartSqlMapperExtensions
	{
		#region async

		public static Task<int> ExecuteAsync(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.ExecuteAsync(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static Task<T> ExecuteScalarAsync<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.ExecuteScalarAsync<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static Task<IEnumerable<T>> QueryAsync<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.QueryAsync<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static Task<IEnumerable<T>> QueryAsync<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params, DataSourceChoice sourceChoice)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.QueryAsync<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			}, sourceChoice);
		}

		public static Task<T> QuerySingleAsync<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.QuerySingleAsync<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static Task<T> QuerySingleAsync<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params, DataSourceChoice sourceChoice)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.QuerySingleAsync<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			}, sourceChoice);
		}

		#endregion async

		#region sync

		public static int Execute(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.Execute(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static T ExecuteScalar<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.ExecuteScalar<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static IEnumerable<T> Query<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.Query<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static IEnumerable<T> Query<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params, DataSourceChoice sourceChoice)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.Query<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			}, sourceChoice);
		}

		public static T QuerySingle<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.QuerySingle<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			});
		}

		public static T QuerySingle<T>(this ISmartSqlMapper sqlMapper, string fullSqlId, dynamic @params, DataSourceChoice sourceChoice)
		{
			EnsurePoint(ref fullSqlId);

			return sqlMapper.QuerySingle<T>(new RequestContext()
			{
				Scope = fullSqlId.Split('.')[0],
				SqlId = fullSqlId.Split('.')[1],
				Request = @params
			}, sourceChoice);
		}

		#endregion sync

		private static void EnsurePoint(ref string fullSqlId)
		{
			if (!fullSqlId.Contains(".")) fullSqlId = "." + fullSqlId;
		}
	}
}