using IndexBuilder.Data.Output;

namespace IndexBuilder.Interfaces
{
	internal interface IWordDefinitionsSerializer
	{
		string Serialize(string language, WordDefinitionsData wordDefinitionsData);
	}
}
