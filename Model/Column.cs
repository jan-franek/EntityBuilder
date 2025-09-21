namespace EntityBuilder.Model
{
	internal class Column
	{
		public string name, dataType;
		public bool nullable, fk, include = true;
		public int length;
		public int precision, scale;

		public string propertyName, propertyType;
		public bool passEnumValue = false;
		public string dafaultValue = null;

		public static Column CreateInstance(System.Data.DataRow row)
		{
			return new Column
			{
				name = Convert.ToString(row["NAME"]),
				dataType = Convert.ToString(row["typename"]),
				nullable = Convert.ToBoolean(row["is_nullable"]),
				fk = row["isFK"] != DBNull.Value,
				length = Convert.ToInt32(row["max_length"]),
				precision = Convert.ToInt32(row["precision"]),
				scale = Convert.ToInt32(row["scale"]),
			};
		}

		public string SqlDataTypeFull
		{
			get
			{
				string suffix = string.Empty;
				switch (dataType)
				{
					case "decimal":
					case "numeric": suffix = string.Format("({0}, {1})", precision, scale); break;
					case "varchar":
					case "nvarchar":
					case "char": suffix = (length < 0) ? "(MAX)" : "(" + length + ")"; break;
				}

				return dataType + suffix;
			}
		}

		public string getPropertyType()
		{
			return dataType switch
			{
				"decimal" or "numeric" => (scale <= 0) ? "int" : "double",
				"int" or "smallint" => "int",
				"tinyint" => "byte",
				"bigint" => "long",
				"smallmoney" or "money" => "decimal",
				"float" => "float",
				"varchar" or "nvarchar" or "text" or "char" => (length == 1) ? "char" : "string",
				"bool" or "bit" => "bool",
				"datetime2" or "datetime" or "date" => "DateTime",
				"time" => "TimeSpan",
				"xml" or "json" => "string",
				"uniqueidentifier" => "Guid",
				"varbinary" => "byte[]",
				_ => throw new NotImplementedException("dataType: " + dataType),
			};
		}

		public string getDefaultValue()
		{

			return propertyType switch
			{
				"int" or "long" => fk ? "-1" : "Int32.MinValue",
				"float" => "float.MinValue",
				"double" => "double.MinValue",
				"decimal" => "decimal.MinValue",
				"char" => "''",
				"string" or "DateTime" or "TimeSpan" => "null",
				"bool" => "false",
				"Guid" => "default(Guid)",
				"byte[]" => "null",
				"byte" => "0",
				_ => throw new NotImplementedException("propertyType: " + propertyType),
			};
		}

		public bool IsSystemType()
		{
			switch (propertyType)
			{
				case "int":
				case "long":
				case "string":
				case "DateTime":
				case "TimeSpan":
				case "double":
				case "float":
				case "decimal":
				case "char":
				case "bool":
				case "byte":
				case "Guid": return true;
				default: return false;
			}
		}

		public void WritePropertyDefinition(StreamWriter fw)
		{
			// attribute
			fw.Write("\t\t[EntityColumn(\"" + name + "\"");
			if (nullable) fw.Write(", true, " + dafaultValue);
			fw.WriteLine(")]");

			// attribute validator
			string validator = GetValidatorDefinition();
			if (!string.IsNullOrEmpty(validator)) fw.WriteLine("\t\t[" + validator + "]");

			// property
			fw.WriteLine($"\t\tpublic {propertyType} {propertyName} {{ get; set; }}{(string.IsNullOrEmpty(dafaultValue) ? "" : $" = {dafaultValue};")}");
		}

		private string GetValidatorDefinition()
		{
			switch (propertyType)
			{
				case "int":
				case "long":
					if (!fk) return null;
					return nullable ? "IntegerValidator(MinValue = -1)" : "IntegerValidator(MinValue = 1)";

				case "string":
					if (nullable)
					{
						return length <= 0 ? null : "StringValidator(MaxLength = " + length + ")";
					}
					return "StringValidator(MinLength = 0, MaxLength = " + length + ")";

				case "DateTime":
				case "TimeSpan":
				case "bool":
				default: return null;
			}
		}
	}
}
