namespace EntityBuilder.Model
{
	internal class Entity
	{
		public string tableName, tableSchema, primaryKey;

		public int idObject;

		public string name, namespaceName, baseClass;

		public void WriteSummary()
		{
			Console.WriteLine("Class name: " + namespaceName + "." + name);
			Console.WriteLine("Table name: " + tableSchema + "." + tableName);
			Console.WriteLine("Base class: " + baseClass);
		}

		public void WriteClassDefinition(StreamWriter fw)
		{
			fw.Write("\t[EntityTable(\"{0}\", \"{1}\"", tableName, tableSchema);

			if (primaryKey != "ID") fw.Write(", PrimaryKeyColumn=\"{0}\"", primaryKey);

			fw.WriteLine(")]");
			fw.WriteLine("\tpublic class " + name + " : " + baseClass);
		}
	}
}
