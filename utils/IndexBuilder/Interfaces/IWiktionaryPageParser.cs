using System.Collections.Generic;
using IndexBuilder.Data;

namespace IndexBuilder.Interfaces
{
	internal interface IWiktionaryPageParser
	{
		IEnumerable<LanguageWordData> ParseLanguageWords(string wikiText);

		IEnumerable<WordDefinition> ParseWordDefinitions(int id, LanguageWordData languageWordData);
	}
}
