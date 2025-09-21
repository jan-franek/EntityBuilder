using System.Data;

namespace EntityBuilder.Data
{
	public interface IDbContext
	{
		object ExecuteScalar(string query, IDictionary<string, object> parameters);
		DataTable ExecuteQuery(string query, IDictionary<string, object> parameters);
	}
}
