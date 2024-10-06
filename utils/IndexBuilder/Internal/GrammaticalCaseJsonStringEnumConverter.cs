using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using IndexBuilder.Model;

namespace IndexBuilder.Internal
{
	internal sealed class GrammaticalCaseJsonStringEnumConverter : JsonConverter<GrammaticalCase>
	{
		private readonly Dictionary<GrammaticalCase, string> caseValues = new();

		public GrammaticalCaseJsonStringEnumConverter(string language)
		{
			var caseTranslations = GetGrammaticalCaseTranslations(language);
			foreach (var caseTranslation in caseTranslations)
			{
				caseValues.Add(caseTranslation.Case, caseTranslation.TranslatedCase);
			}
		}

		public override GrammaticalCase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, GrammaticalCase value, JsonSerializerOptions options)
		{
			if (!caseValues.TryGetValue(value, out var caseTranslation))
			{
				throw new NotSupportedException($"Grammatical case is not supported: {value}");
			}

			writer.WriteStringValue(caseTranslation);
		}

		private static IEnumerable<(GrammaticalCase Case, string TranslatedCase)> GetGrammaticalCaseTranslations(string language)
		{
			if (language == "polski")
			{
				yield return (GrammaticalCase.Nominative, "mianownik");
				yield return (GrammaticalCase.Genitive, "dopełniacz");
				yield return (GrammaticalCase.Dative, "celownik");
				yield return (GrammaticalCase.Accusative, "biernik");
				yield return (GrammaticalCase.Instrumental, "narzędnik");
				yield return (GrammaticalCase.Locative, "miejscownik");
				yield return (GrammaticalCase.Vocative, "wołacz");
				yield break;
			}

			throw new NotSupportedException($"Language is not supported: '{language}'");
		}
	}
}
