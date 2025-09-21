using EntityBuilder.Data;
using System.Configuration;

namespace EntityBuilder
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string defaultSchema = ConfigurationManager.AppSettings["defaultSchema"];
			string defaultNamespace = ConfigurationManager.AppSettings["defaultNamespace"];

			string schema = args.Length >= 1 ? args[0] : defaultSchema;
			string @namespace = args.Length >= 2 ? args[1] : defaultNamespace;

			try
			{
#if DEBUG
				var entityBuilder = new EntityBuilder(new FakeDbContext());
#else
				var entityBuilder = new EntityBuilder(new SqlServerDbContext(ConfigurationManager.ConnectionStrings["db"].ConnectionString));
#endif
				entityBuilder.GenerateEntity(schema, @namespace);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occured:\n" + ex);
				Console.ReadLine();
				return;
			}
		}
	}
}
