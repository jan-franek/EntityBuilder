using System.Data;

namespace EntityBuilder.Data
{
	internal class FakeDbContext : IDbContext
	{
		public object ExecuteScalar(string query, IDictionary<string, object> parameters)
		{
			// Always return a fake object_id
			return 1;
		}

		public DataTable ExecuteQuery(string query, IDictionary<string, object> parameters)
		{
			var table = new DataTable();
			table.Columns.Add("name");
			table.Columns.Add("system_type_id");
			table.Columns.Add("max_length");
			table.Columns.Add("precision");
			table.Columns.Add("scale");
			table.Columns.Add("is_nullable");
			table.Columns.Add("isFK");
			table.Columns.Add("typeName");

			// Add fake columns
			table.Rows.Add("ID", 56, 4, 10, 0, false, null, "int");
			table.Rows.Add("COMPANYNAME", 167, 100, 0, 0, true, null, "varchar");
			table.Rows.Add("PAYMENTBALANCESIZE", 56, 4, 3, 0, true, null, "int");

			return table;
		}
	}
}
