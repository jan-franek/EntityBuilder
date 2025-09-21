namespace EntityBuilder.Helpers
{
	internal static class ConsoleHelper
	{
		public static string ReadString(string name)
		{
			Console.Write(name + ": ");

			return Console.ReadLine().Trim();
		}

		public static string ReadString(string name, string defValue)
		{
			Console.Write(name + " (" + defValue + "): ");

			string input = Console.ReadLine()?.Trim();

			return string.IsNullOrEmpty(input) ? defValue : input;
		}

		public static int ReadInt(string name)
		{
			Console.Write(name + ": ");

			while (true)
			{
				string input = Console.ReadLine()?.Trim();

				if (int.TryParse(input, out var i)) return i;
			}
		}

		public static bool ReadBool(string name, bool defaultValue)
		{
			if (!string.IsNullOrEmpty(name)) Console.Write(name + " (" + defaultValue + "): ");

			while (true)
			{
				string input = Console.ReadLine()?.Trim();

				switch (input?.ToLower())
				{
					case "0":
					case "f":
					case "n":
					case "ne":
					case "no":
					case "false":
						return false;

					case "1":
					case "t":
					case "y":
					case "a":
					case "ano":
					case "yes":
					case "true":
						return true;

					case null:
					case "":
						return defaultValue;
				}
			}
		}
	}
}
