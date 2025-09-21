using EntityBuilder.Data;
using EntityBuilder.Helpers;
using EntityBuilder.Model;
using System.Text;

namespace EntityBuilder
{
	public class EntityBuilder
	{
		private readonly IDbContext _db;

		public EntityBuilder(IDbContext dbContext)
		{
			_db = dbContext;
		}

		public void GenerateEntity(string tableSchema, string tableNamespace)
		{
			var entity = new Entity
			{
				tableSchema = ConsoleHelper.ReadString("Table schema", tableSchema),
				tableName = ConsoleHelper.ReadString("Table name")
			};

			entity.idObject = GetEntityID(entity.tableSchema, entity.tableName);
			if (entity.idObject <= 0)
			{
				Console.WriteLine("ERROR: entity couldn't be found");
				return;
			}

			Console.WriteLine("Entity ID: " + entity.idObject);

			Console.Write("Reading column definition ... ");
			Column[] cols = GetColumnDefinitions(entity.idObject);

			Console.WriteLine(cols.Length + " found");
			if (cols.Length <= 0) return;

			FillHeader(entity, cols, tableNamespace);

			try
			{
				FillColumns(entity, cols);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occured:\n" + ex);
				Console.ReadLine();
			}

			Console.WriteLine("\nEntity summary\n----------------------");
			entity.WriteSummary();

			Console.WriteLine("\nActions\n----------------------");

			GenerateClass(entity, cols);

			Console.WriteLine("Complete");
		}

		#region private methods

		private int GetEntityID(string schema, string name)
		{
			var parameters = new Dictionary<string, object>
			{
				["@objectName"] = $"{schema}.{name}"
			};

			var result = _db.ExecuteScalar("select object_id(@objectName)", parameters);
			return result != null ? Convert.ToInt32(result) : -1;
		}

		private Column[] GetColumnDefinitions(int idEntity)
		{
			var sql = new StringBuilder();

			sql.Append(" select name, system_type_id, max_length, [precision], scale, is_nullable, ");
			sql.Append("        (SELECT 1 from sys.foreign_key_columns where parent_object_id = col.object_id and parent_column_id = col.column_id) isFK, ");
			sql.Append("        (SELECT TOP 1 name from sys.types where system_type_id = col.system_type_id) typeName ");
			sql.Append("   from sys.columns col ");
			sql.Append("  where col.object_id = @idEntity ");
			sql.Append("  order by col.column_id ");

			var parameters = new Dictionary<string, object>
			{
				["@idEntity"] = idEntity
			};

			var table = _db.ExecuteQuery(sql.ToString(), parameters);
			if (table.Rows.Count <= 0) return [];

			var columns = new Column[table.Rows.Count];
			for (int i = 0; i < table.Rows.Count; i++) columns[i] = Column.CreateInstance(table.Rows[i]);

			return columns;
		}

		private static void FillHeader(Entity e, Column[] cols, string ns)
		{
			Console.WriteLine("\nEntity header\n----------------------");

			e.namespaceName = ConsoleHelper.ReadString("Namespace", ns);

			e.name = e.tableName;
			e.name = ConsoleHelper.ReadString("Entity name", e.name);

			e.baseClass = ConsoleHelper.ReadString("Base class", "BaseEntity");

			e.primaryKey = ConsoleHelper.ReadString("Primary key column", cols[0].name);
		}

		private static void FillColumns(Entity e, Column[] cols)
		{
			Console.WriteLine("\nColumns\n----------------------");

			foreach (Column c in cols)
			{
				if (c.name.Equals(e.primaryKey))
				{
					c.include = false;
					continue;
				}

				FillColumn(e, c);
			}
		}

		private static void FillColumn(Entity e, Column c)
		{
			c.propertyName = Capitalizator.Capitalize(c.name);
			c.propertyType = c.getPropertyType();
			Console.Write("  {1} {0}, correct: ", c.propertyName, c.propertyType);

			string s = ConsoleHelper.ReadString(null, string.Empty);
			if (!string.IsNullOrEmpty(s))
			{
				if (!string.Equals(s, "f", StringComparison.OrdinalIgnoreCase)) c.propertyName = s;
				else c.propertyName = ConsoleHelper.ReadString("\tName", c.propertyName);

				c.propertyType = ConsoleHelper.ReadString("\tType", c.propertyType);
			}

			if (!c.IsSystemType()) c.passEnumValue = ConsoleHelper.ReadBool("\tPass enumeration as value", true);
			if (c.nullable) c.dafaultValue = ConsoleHelper.ReadString("\tDefault value", c.getDefaultValue());
		}

		private static void GenerateClass(Entity e, Column[] cols)
		{
			Console.WriteLine("\nGenerating class file\n----------------------");

			string fileName = e.name + ".cs";
			Console.WriteLine("  FileName: " + fileName);

			if (!Directory.Exists("entities")) Directory.CreateDirectory("entities");

			using (var fw = new StreamWriter("entities\\" + fileName, false, Encoding.UTF8))
			{
				fw.WriteLine("using cz.InterDream.Data.Entities;");
				fw.WriteLine("using System;");
				fw.WriteLine("using System.Collections.Generic;");
				fw.WriteLine("using System.Text;");
				fw.WriteLine("using System.Data;");
				fw.WriteLine("using System.Configuration;");

				fw.WriteLine("\nnamespace " + e.namespaceName + "\n{");

				e.WriteClassDefinition(fw);
				fw.WriteLine("\t{");

				var firstColumn = true;
				foreach (Column c in cols)
				{
					if (!c.include) continue;

					if (firstColumn) firstColumn = false;
					else fw.WriteLine();

					c.WritePropertyDefinition(fw);
				}

				fw.WriteLine("\t}");

				fw.WriteLine("}");
			}
		}

		#endregion
	}
}
