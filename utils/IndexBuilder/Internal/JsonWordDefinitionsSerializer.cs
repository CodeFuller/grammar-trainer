using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using IndexBuilder.Data.Output;
using IndexBuilder.Interfaces;

namespace IndexBuilder.Internal
{
	internal class JsonWordDefinitionsSerializer : IWordDefinitionsSerializer
	{
		public string Serialize(string language, WordDefinitionsData wordDefinitionsData)
		{
#pragma warning disable CA1869 // JsonSerializerOptions depends on input parameter (language).
			var jsonSerializerOptions = new JsonSerializerOptions
			{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				Converters =
				{
					new GrammaticalCaseJsonStringEnumConverter(language),
					new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
				},
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			};
#pragma warning restore CA1869

			return JsonSerializer.Serialize(wordDefinitionsData, jsonSerializerOptions);
		}
	}
}
