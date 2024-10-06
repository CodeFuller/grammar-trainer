using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IndexBuilder.Data.Input;
using IndexBuilder.Interfaces;
using IndexBuilder.Model;
using IndexBuilder.WikitextParsing;

namespace IndexBuilder.Internal
{
	internal sealed class WiktionaryPageParser : IWiktionaryPageParser
	{
		private static readonly Regex LanguageRegex = new("^język (.+)$", RegexOptions.Compiled);

		private static readonly Regex LanguageSpecificTitleRegex = new(@"^==\s*(.+?) \({{(.+?)}}\)\s+==\s*$", RegexOptions.Compiled);

		private static readonly Regex ChineseCharacterTitleRegex = new(@"^== ({{zh|(.+?)}}) \({{znak chiński}}\) ==$", RegexOptions.Compiled);

		private static readonly Regex InternationTitleRegex = new(@"^== (.+?) \({{użycie międzynarodowe}}\) ?==$", RegexOptions.Compiled);

		private static readonly Regex NumberTitleRegex = new(@"^== ((?:\d+ )*\d+) ==$", RegexOptions.Compiled);

		private static readonly Regex DictionaryTitleRegex = new(@"^\|słownik=(Słownik .+)$", RegexOptions.Compiled);

		private static readonly Regex NounDeclensionSectionStartRegex = new(@"{{odmiana-rzeczownik-polski\s*", RegexOptions.Compiled);

		private static readonly Regex IndeclinableNounRegex = new("{{nieodm-rzeczownik-polski}}", RegexOptions.Compiled);

		private static readonly Regex NounFormRegex = new(@"^\|\s*(.+?) (l[pm])\s*=\s*(.*)$", RegexOptions.Compiled);
		private static readonly Regex CheckedFormRegex = new(@"^\|\s*sprawdzone\s*=\s*tak$", RegexOptions.Compiled);
		private static readonly Regex DepreciativeFormRegex = new(@"^\|\s*Forma depr\s*=\s*.*$", RegexOptions.Compiled);
		private static readonly Regex NonDepreciativeFormRegex = new(@"^\|\s*Forma ndepr\s*=\s*.*$", RegexOptions.Compiled);

		private static readonly Dictionary<string, GrammaticalCase> GrammaticalCases = new()
		{
			{ "Mianownik", GrammaticalCase.Nominative },
			{ "Dopełniacz", GrammaticalCase.Genitive },
			{ "Celownik", GrammaticalCase.Dative },
			{ "Biernik", GrammaticalCase.Accusative },
			{ "Narzędnik", GrammaticalCase.Instrumental },
			{ "Miejscownik", GrammaticalCase.Locative },
			{ "Wołacz", GrammaticalCase.Vocative },
		};

		private readonly IWikitextParser wikitextParser;

		public WiktionaryPageParser(IWikitextParser wikitextParser)
		{
			this.wikitextParser = wikitextParser ?? throw new ArgumentNullException(nameof(wikitextParser));
		}

		private sealed class CurrentWordContext
		{
			private readonly StringBuilder wikitextBulder = new();

			public string Language { get; }

			public string Word { get; }

			public CurrentWordContext(string language, string word)
			{
				Language = language;
				Word = word;
			}

			public LanguageWordData ToLanguageWordData()
			{
				return new LanguageWordData
				{
					Language = Language,
					Word = Word,
					Wikitext = wikitextBulder.ToString(),
				};
			}

			public void AppendWikitextLine(string wikitextLine)
			{
				wikitextBulder.AppendLine(wikitextLine);
			}
		}

		public IEnumerable<LanguageWordData> ParseLanguageWords(string wikiText)
		{
			var textLines = SplitByLines(wikiText);

			CurrentWordContext currentWordContext = null;

			foreach (var line in textLines)
			{
				if (TryParseWordTitle(line, out var wordData))
				{
					if (currentWordContext != null)
					{
						yield return currentWordContext.ToLanguageWordData();
					}

					currentWordContext = new CurrentWordContext(wordData.Language, wordData.Word);
				}

				currentWordContext?.AppendWikitextLine(line);
			}

			if (currentWordContext != null)
			{
				yield return currentWordContext.ToLanguageWordData();
			}
		}

		private static bool TryParseWordTitle(string line, out (string Language, string Word) data)
		{
			var match = LanguageSpecificTitleRegex.Match(line);
			if (match.Success)
			{
				data = (Language: ExtractLanguageName(match.Groups[2].Value), Word: match.Groups[1].Value);
				return true;
			}

			match = ChineseCharacterTitleRegex.Match(line);
			if (match.Success)
			{
				data = (Language: "chiński", Word: match.Groups[1].Value);
				return true;
			}

			match = InternationTitleRegex.Match(line);
			if (match.Success)
			{
				data = (Language: "international", Word: match.Groups[1].Value);
				return true;
			}

			match = NumberTitleRegex.Match(line);
			if (match.Success)
			{
				data = (Language: "international", Word: match.Groups[1].Value);
				return true;
			}

			match = DictionaryTitleRegex.Match(line);
			if (match.Success)
			{
				data = (Language: null, Word: match.Groups[1].Value);
				return true;
			}

			data = default;
			return false;
		}

		private static string ExtractLanguageName(string languageTitle)
		{
			var match = LanguageRegex.Match(languageTitle);
			return match.Success ? match.Groups[1].Value : languageTitle;
		}

		private enum ParsingState
		{
			None,
			NounDeclensionSection,
		}

		public IEnumerable<WordDefinition> ParseWordDefinitions(int id, LanguageWordData languageWordData)
		{
			var textLines = SplitByLines(languageWordData.Wikitext);

			var parsingState = ParsingState.None;
			var wordFormLines = new List<string>();

			foreach (var line in textLines)
			{
				switch (parsingState)
				{
					case ParsingState.None:
						if (NounDeclensionSectionStartRegex.IsMatch(line))
						{
							parsingState = ParsingState.NounDeclensionSection;
						}
						else if (IndeclinableNounRegex.IsMatch(line))
						{
							yield return CreateIndeclinableNounDefinition(languageWordData.Word);
						}

						break;

					case ParsingState.NounDeclensionSection:
						if (line == "}}")
						{
							yield return CreateNounDefinition(wordFormLines);

							wordFormLines.Clear();
							parsingState = ParsingState.None;
						}
						else if (line == "}} ''lub'' {{odmiana-rzeczownik-polski" || line == "}} lub {{odmiana-rzeczownik-polski")
						{
							yield return CreateNounDefinition(wordFormLines);

							wordFormLines.Clear();
							parsingState = ParsingState.NounDeclensionSection;
						}
						else if (line == "}} ''lub'' {{nieodm-rzeczownik-polski}}" || line == "}} lub {{nieodm-rzeczownik-polski}}")
						{
							yield return CreateNounDefinition(wordFormLines);
							yield return CreateIndeclinableNounDefinition(languageWordData.Word);

							wordFormLines.Clear();
							parsingState = ParsingState.None;
						}
						else
						{
							wordFormLines.Add(line);
						}

						break;

					default:
						throw new NotSupportedException($"Parsing state is not supported: {parsingState}");
				}
			}
		}

		private NounDefinition CreateNounDefinition(List<string> wordFormLines)
		{
			var formsSet = new HashSet<(GrammaticalCase Case, GrammaticalNumber Number)>();
			var forms = new List<NounDeclensionForm>();

			foreach (var line in wordFormLines)
			{
				var match = NounFormRegex.Match(line);
				if (!match.Success)
				{
					if (CheckedFormRegex.IsMatch(line) || DepreciativeFormRegex.IsMatch(line) || NonDepreciativeFormRegex.IsMatch(line))
					{
						continue;
					}

					throw new InvalidOperationException($"Failed to parse noun form from line '{line}'");
				}

				var caseString = match.Groups[1].Value;
				if (!GrammaticalCases.TryGetValue(caseString, out var formCase))
				{
					throw new InvalidOperationException($"Failed to extract grammatical case from line '{line}'");
				}

				var formNumber = match.Groups[2].Value == "lp" ? GrammaticalNumber.Singular : GrammaticalNumber.Plural;

				if (!formsSet.Add((Case: formCase, Number: formNumber)))
				{
					throw new InvalidOperationException($"Detected duplicated word form in line {line}");
				}

				var formText = match.Groups[3].Value;
				if (formText.Length == 0)
				{
					continue;
				}

				var form = new NounDeclensionForm
				{
					Case = formCase,
					Number = formNumber,
					FormValues = wikitextParser.ParseFormValues(formText).ToList(),
				};

				forms.Add(form);
			}

			return new NounDefinition
			{
				Forms = forms.Order(new NounDeclensionFormComparer()).ToList(),
			};
		}

		private static NounDefinition CreateIndeclinableNounDefinition(string word)
		{
			return new NounDefinition
			{
				Forms = GetAllPossibleForms().Select(x => new NounDeclensionForm
					{
						Case = x.Case,
						Number = x.Number,
						FormValues =
						[
							new NounDeclensionFormValue
							{
								FormValue = word,
								Flags = DeclensionFormFlags.None,
							},
						],
					})
					.ToList(),
			};
		}

		private static IEnumerable<(GrammaticalCase Case, GrammaticalNumber Number)> GetAllPossibleForms()
		{
			yield return (GrammaticalCase.Nominative, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Genitive, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Dative, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Accusative, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Instrumental, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Locative, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Vocative, GrammaticalNumber.Singular);
			yield return (GrammaticalCase.Nominative, GrammaticalNumber.Plural);
			yield return (GrammaticalCase.Genitive, GrammaticalNumber.Plural);
			yield return (GrammaticalCase.Dative, GrammaticalNumber.Plural);
			yield return (GrammaticalCase.Accusative, GrammaticalNumber.Plural);
			yield return (GrammaticalCase.Instrumental, GrammaticalNumber.Plural);
			yield return (GrammaticalCase.Locative, GrammaticalNumber.Plural);
			yield return (GrammaticalCase.Vocative, GrammaticalNumber.Plural);
		}

		private static string[] SplitByLines(string str)
		{
			return str.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
		}
	}
}
