using System;
using System.Collections.Generic;
using System.Linq;
using IndexBuilder.Model;
using IndexBuilder.WikitextParsing.Nodes;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace IndexBuilder.WikitextParsing
{
	// Grammar:
	//   FormValues:                FormValue / FormValue Delimiter FormValues / "{{potencjalnie|" FormValues "}}"
	//   FormValue:                 Flags FormValueWithoutFlags Flags
	//   FormValueWithoutFlags:     WordOrWikilink References? / "{{potencjalnie|" FormValue "}}"
	//   Flags:                     '' | ({{'a-z'}} / SpecificPrepositionFlag) References? Flags
	//   SpecificPrepositionFlag:   ''tylko po "ku"''
	//   Preposition:               \w+
	//   WordOrWikilink:            Word | "[[" Word "]]"
	//   Word:                      [\w '’.!-]+
	internal class PidginFormValuesParser : IFormValuesParser
	{
		private static readonly Parser<char, string> UnpairedReferenceTagParser = TokenParser(String("<ref ").Then(Any.SkipManyThen(String("/>"))));
		private static readonly Parser<char, string> PairedReferenceTagParser = TokenParser(String("<ref").Then(Char('>').Or(Char(' '))).Then(Any.SkipManyThen(String("</ref>"))));
		private static readonly Parser<char, string> ReferenceParser = OneOf(Try(UnpairedReferenceTagParser), PairedReferenceTagParser);
		private static readonly Parser<char, IEnumerable<string>> OptionalReferencesParser = ReferenceParser.Many();

		// Special case for single apostrophe is necessary to distinguish delimiter ''lub''.
		private static readonly Parser<char, string> NonWhitespaceCharParser =
			Try(Letter.Then(Char('\''), (c1, c2) => new String(new[] { c1, c2 })))
				.Or(Letter.Select(c => new String(c, 1)))
				.Or(String("’"))
				.Or(String("."))
				.Or(String("!"))
				.Or(String("-"));

		private static readonly Parser<char, string> SpaceAndNonWhitespaceCharParser = Whitespace.Then(NonWhitespaceCharParser, (c, s) => c + s);
		private static readonly Parser<char, string> WordParser = Try(Try(SpaceAndNonWhitespaceCharParser).Or(NonWhitespaceCharParser.AtLeastOnceString()).AtLeastOnceString());
		private static readonly Parser<char, FormValueNode> WordOrWikilinkParser = TokenParser(WordParser.Between(String("[["), String("]]")).Or(WordParser)).Select(x => new FormValueNode(x));
		private static readonly Parser<char, FormValueNode> WordOrWikilinkWithOptionalReferencesParser = WordOrWikilinkParser.Before(OptionalReferencesParser);

		private static readonly Parser<char, string> FlagValueParser = Letter.ManyString();
		private static readonly Parser<char, string> PrepositionParser = Letter.AtLeastOnceString();

		private static readonly Parser<char, string> SpecificPrepositionParser =
			OneOf(
					Try(PrepositionParser.Between(String("''(tylko po „"), String("”)''"))),
					Try(PrepositionParser.Between(String("''tylko po „[["), String("]]”''"))),
					Try(PrepositionParser.Between(String("''tylko po \""), String("\"''"))),
					PrepositionParser.Between(String("''tylko po „"), String("”''")))
				.Select(_ => "specific-preposition");

		private static readonly Parser<char, string> FlagTemplateParser = TokenParser(
			OneOf(
					FlagValueParser.Between(String("{{"), String("}}")),
					SpecificPrepositionParser)
				.Before(OptionalReferencesParser));

		private static readonly Parser<char, string> DelimiterParser = TokenParser(
			String("/")
				.Or(String(","))
				.Or(Try(String("<br/>")))
				.Or(Try(String("<br />")))
				.Or(Try(String("<br>")))
				.Or(Try(String("''lub''"))));

		private static readonly Parser<char, FormValueNode> FormValueParser =
			FlagTemplateParser.Many()
				.Then(Rec(() => FormValueWithoutFlagsParser), (flags, formValueNode) => formValueNode.WithFlags(flags))
				.Then(FlagTemplateParser.Many(), (formValueNode, flags) => formValueNode.WithFlags(flags));

		private static readonly Parser<char, FormValueNode> FormValueWithoutFlagsParser = OneOf(
				Try(WordOrWikilinkWithOptionalReferencesParser),
				PotentialTemplateParser(Rec(() => FormValueParser)).Select(x => x.WithFlag(DeclensionFormFlags.Potential)));

		private static readonly Parser<char, FormValuesNode> FormValuesParser = OneOf(
				Try(Map((formValue, _, formValues) => formValue + formValues, FormValueParser, DelimiterParser, Rec(() => FormValuesParser))),
				PotentialTemplateParser(Rec(() => FormValuesParser)).Select(x => x.WithFlag(DeclensionFormFlags.Potential)),
				FormValueParser.Select(x => new FormValuesNode(x)));

		private static readonly Parser<char, FormValuesNode> WikitextFormValuesParserParser = FormValuesParser.Before(End);

		private static Parser<char, T> TokenParser<T>(Parser<char, T> token)
		{
			return Try(token).Before(SkipWhitespaces);
		}

		private static Parser<char, T> PotentialTemplateParser<T>(Parser<char, T> parser)
		{
			return TokenParser(parser.Between(TokenParser(String("{{potencjalnie|")), String("}}")).Before(OptionalReferencesParser));
		}

		public IEnumerable<NounDeclensionFormValue> ParseFormValues(string wikitext)
		{
			var parseResult = WikitextFormValuesParserParser.Parse(wikitext);
			if (!parseResult.Success)
			{
				throw new InvalidOperationException($"Failed to parse form values from wikitext '{wikitext}'");
			}

			return parseResult.Value.FormValues.Select(x => new NounDeclensionFormValue
				{
					FormValue = x.Word,
					Flags = x.Flags,
				})
				.ToList();
		}
	}
}
