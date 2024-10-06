using System.Collections.Generic;
using IndexBuilder.Model;

namespace IndexBuilder.WikitextParsing
{
	internal interface IWikitextParser
	{
		string ParseWordFromTitle(string wikitext);

		IEnumerable<NounDeclensionFormValue> ParseFormValues(string wikitext);
	}
}
