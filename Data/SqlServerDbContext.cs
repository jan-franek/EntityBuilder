using Microsoft.Data.SqlClient;
using System.Data;

namespace EntityBuilder.Data
{
	internal class SqlServerDbContext : IDbContext
	{
		private readonly string _connectionString;

		public SqlServerDbContext(string connectionString)
		{
			_connectionString = connectionString;
		}

		public object ExecuteScalar(string query, IDictionary<string, object> parameters)
		{
			using var connection = new SqlConnection(_connectionString);
			using var command = new SqlCommand(query, connection);

			foreach (var param in parameters) command.Parameters.AddWithValue(param.Key, param.Value);

			connection.Open();
			return command.ExecuteScalar();
		}

		public DataTable ExecuteQuery(string query, IDictionary<string, object> parameters)
		{
			using var connection = new SqlConnection(_connectionString);
			using var command = new SqlCommand(query, connection);

			foreach (var param in parameters) command.Parameters.AddWithValue(param.Key, param.Value);

			var table = new DataTable();
			using var adapter = new SqlDataAdapter(command);
			adapter.Fill(table);

			return table;
		}
	}
}
