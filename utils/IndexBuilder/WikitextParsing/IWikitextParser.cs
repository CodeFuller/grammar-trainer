using System.Collections.Generic;
using IndexBuilder.Model;

namespace IndexBuilder.WikitextParsing
{
	internal interface IWikitextParser
	{
		IEnumerable<NounDeclensionFormValue> ParseFormValues(string wikitext);
	}
}
