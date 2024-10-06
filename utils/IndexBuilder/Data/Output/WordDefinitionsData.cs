using System.Collections.Generic;
using System.Linq;
using IndexBuilder.Model;

namespace IndexBuilder.Data.Output
{
	internal class WordDefinitionsData
	{
		private readonly List<NounDefinitionData> nouns = new();

		public IReadOnlyCollection<NounDefinitionData> Nouns => nouns;

		public void Add(string word, IReadOnlyCollection<WordDefinition> wordDefinitions)
		{
			var nounDefinitions = wordDefinitions.OfType<NounDefinition>().ToList();
			if (nounDefinitions.Count != 0)
			{
				nouns.Add(new NounDefinitionData
				{
					Word = word,
					Definitions = nounDefinitions,
				});
			}
		}
	}
}
