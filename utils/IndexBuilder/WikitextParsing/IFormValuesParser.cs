using System.Collections.Generic;
using IndexBuilder.Model;

namespace IndexBuilder.WikitextParsing
{
	internal interface IFormValuesParser
	{
		IEnumerable<NounDeclensionFormValue> ParseFormValues(string wikitext);
	}
}
