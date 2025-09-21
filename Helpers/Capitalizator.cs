using System.Text;

namespace EntityBuilder.Helpers
{
	internal static class Capitalizator
	{
		private static readonly string[] stopWords = null;

		static Capitalizator()
		{
			stopWords = LoadStopWords();
		}

		private static string[] LoadStopWords()
		{
			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stopwords.dat");
			if (!File.Exists(file))
			{
				Console.WriteLine("WARNING: Couldn't load stopwords.dat, names of Entity's properties won't be capitalized");
				return null;
			}

			var result = new List<string>();

			using (var fr = new StreamReader(file, true))
			{
				string line = null;

				while ((line = fr.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(line)) continue;
					if (line[0] == '#') continue;

					result.Add(line.ToLower().Trim());
				}
			}

			return [.. result];
		}

		public static string Capitalize(string data)
		{
			// init
			data = data.ToLower();
			if (stopWords is null) return data;

			var capitalizedData = new StringBuilder(data);
			var capitalizedLetters = new bool[data.Length];
			var lettersLeftToCapitalize = data.Length;

			// first letter
			var startIndex = 1;
			capitalizedLetters[0] = true;
			capitalizedData[0] = char.ToUpper(capitalizedData[0]);
			lettersLeftToCapitalize--;

			// starts with ID
			if (data.StartsWith("id"))
			{
				capitalizedData[1] = 'D';
				capitalizedLetters[1] = true;
				startIndex = 2;
				lettersLeftToCapitalize--;

				// just ID
				if (lettersLeftToCapitalize <= 0) return capitalizedData.ToString();
			}

			// find and capitalize stopwords
			foreach (var word in stopWords)
			{
				var wordStart = data.IndexOf(word, startIndex);
				if (wordStart < 0 || capitalizedLetters[wordStart]) continue;

				// check collision - if any letter of the word is already capitalized, skip the stopword
				var collision = false;
				for (var p = 0; p < word.Length; p++)
				{
					if (capitalizedLetters[wordStart + p])
					{
						collision = true;
						break;
					}
				}
				if (collision) continue;

				// capitalize first letter of the word
				capitalizedData[wordStart] = char.ToUpper(capitalizedData[wordStart]);

				// mark letters of the word as capitalized
				for (var p = 0; p < word.Length; p++)
				{
					capitalizedLetters[wordStart + p] = true;
					lettersLeftToCapitalize--;
				}

				if (lettersLeftToCapitalize <= 0) break;
			}

			capitalizedData.Replace("_", "");
			return capitalizedData.ToString();
		}
	}
}