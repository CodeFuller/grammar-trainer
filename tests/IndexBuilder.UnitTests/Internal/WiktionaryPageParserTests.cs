using System.IO;
using System.Linq;
using FluentAssertions;
using IndexBuilder.Data.Input;
using IndexBuilder.Internal;
using IndexBuilder.Model;
using IndexBuilder.WikitextParsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IndexBuilder.Model.GrammaticalCase;
using static IndexBuilder.Model.GrammaticalNumber;
using static IndexBuilder.UnitTests.Helpers.NounDeclensionHelpers;

namespace IndexBuilder.UnitTests.Internal
{
	[TestClass]
	public class WiktionaryPageParserTests
	{
		[TestMethod]
		public void ParseLanguageWords_ForTitleWithWikilinks_ReturnsCorrectWord()
		{
			// Arrange

			var target = CreateTestTarget();

			var wikitext = LoadWikitext("Republika Czeska.txt");

			// Act

			var languageWords = target.ParseLanguageWords(wikitext).ToList();

			// Assert

			var expectedLanguageWords = new[]
			{
				new LanguageWordData
				{
					Language = "polski",
					Word = "Republika Czeska",
				},
			};

			languageWords.Should().BeEquivalentTo(expectedLanguageWords, x => x.Excluding(y => y.Wikitext));
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNoun_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("kobieta")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "kobieta"),
						CreateDeclensionForm(Genitive, Singular, "kobiety"),
						CreateDeclensionForm(Dative, Singular, "kobiecie"),
						CreateDeclensionForm(Accusative, Singular, "kobietę"),
						CreateDeclensionForm(Instrumental, Singular, "kobietą"),
						CreateDeclensionForm(Locative, Singular, "kobiecie"),
						CreateDeclensionForm(Vocative, Singular, "kobieto"),
						CreateDeclensionForm(Nominative, Plural, "kobiety"),
						CreateDeclensionForm(Genitive, Plural, "kobiet"),
						CreateDeclensionForm(Dative, Plural, "kobietom"),
						CreateDeclensionForm(Accusative, Plural, "kobiety"),
						CreateDeclensionForm(Instrumental, Plural, "kobietami"),
						CreateDeclensionForm(Locative, Plural, "kobietach"),
						CreateDeclensionForm(Vocative, Plural, "kobiety"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithReference_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("kawaler")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "kawaler"),
						CreateDeclensionForm(Genitive, Singular, "kawalera"),
						CreateDeclensionForm(Dative, Singular, "kawalerowi"),
						CreateDeclensionForm(Accusative, Singular, "kawalera"),
						CreateDeclensionForm(Instrumental, Singular, "kawalerem"),
						CreateDeclensionForm(Locative, Singular, "kawalerze"),
						CreateDeclensionForm(Vocative, Singular, "kawalerze"),
						CreateDeclensionForm(Nominative, Plural, ["kawalerowie", "kawalerzy"]),
						CreateDeclensionForm(Genitive, Plural, "kawalerów"),
						CreateDeclensionForm(Dative, Plural, "kawalerom"),
						CreateDeclensionForm(Accusative, Plural, "kawalerów"),
						CreateDeclensionForm(Instrumental, Plural, "kawalerami"),
						CreateDeclensionForm(Locative, Plural, "kawalerach"),
						CreateDeclensionForm(Vocative, Plural, ["kawalerowie", "kawalerzy"]),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithMultipleWordFormsSplitBySlashWithSpaces_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("strzelec")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "strzelec"),
						CreateDeclensionForm(Genitive, Singular, "strzelca"),
						CreateDeclensionForm(Dative, Singular, "strzelcowi"),
						CreateDeclensionForm(Accusative, Singular, "strzelca"),
						CreateDeclensionForm(Instrumental, Singular, "strzelcem"),
						CreateDeclensionForm(Locative, Singular, "strzelcu"),
						CreateDeclensionForm(Vocative, Singular, ["strzelcu", "strzelcze"]),
						CreateDeclensionForm(Nominative, Plural, "strzelcy"),
						CreateDeclensionForm(Genitive, Plural, "strzelców"),
						CreateDeclensionForm(Dative, Plural, "strzelcom"),
						CreateDeclensionForm(Accusative, Plural, "strzelców"),
						CreateDeclensionForm(Instrumental, Plural, "strzelcami"),
						CreateDeclensionForm(Locative, Plural, "strzelcach"),
						CreateDeclensionForm(Vocative, Plural, "strzelcy"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithMultipleWordFormsSplitBySlashWithoutSpacesAndPairedRefTag_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("kumkwat")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "kumkwat"),
						CreateDeclensionForm(Genitive, Singular, ["kumkwata", "kumkwatu"]),
						CreateDeclensionForm(Dative, Singular, "kumkwatowi"),
						CreateDeclensionForm(Accusative, Singular, "kumkwat"),
						CreateDeclensionForm(Instrumental, Singular, "kumkwatem"),
						CreateDeclensionForm(Locative, Singular, "kumkwacie"),
						CreateDeclensionForm(Vocative, Singular, "kumkwacie"),
						CreateDeclensionForm(Nominative, Plural, "kumkwaty"),
						CreateDeclensionForm(Genitive, Plural, "kumkwatów"),
						CreateDeclensionForm(Dative, Plural, "kumkwatom"),
						CreateDeclensionForm(Accusative, Plural, "kumkwaty"),
						CreateDeclensionForm(Instrumental, Plural, "kumkwatami"),
						CreateDeclensionForm(Locative, Plural, "kumkwatach"),
						CreateDeclensionForm(Vocative, Plural, "kumkwaty"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithMultipleWordFormsSplitBySlashWithoutSpacesAndNonPairedRefTag_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("kororczyk")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "kororczyk"),
						CreateDeclensionForm(Genitive, Singular, "kororczyka"),
						CreateDeclensionForm(Dative, Singular, "kororczykowi"),
						CreateDeclensionForm(Accusative, Singular, "kororczyka"),
						CreateDeclensionForm(Instrumental, Singular, "kororczykiem"),
						CreateDeclensionForm(Locative, Singular, "kororczyku"),
						CreateDeclensionForm(Vocative, Singular, "kororczyku"),
						CreateDeclensionForm(Nominative, Plural, "kororczycy"),
						CreateDeclensionForm(Genitive, Plural, "kororczyków"),
						CreateDeclensionForm(Dative, Plural, "kororczykom"),
						CreateDeclensionForm(Accusative, Plural, "kororczyków"),
						CreateDeclensionForm(Instrumental, Plural, "kororczykami"),
						CreateDeclensionForm(Locative, Plural, "kororczykach"),
						CreateDeclensionForm(Vocative, Plural, "kororczycy"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithMultipleWordFormsSplitByBrTag_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("kabza")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "kabza"),
						CreateDeclensionForm(Genitive, Singular, "kabzy"),
						CreateDeclensionForm(Dative, Singular, "kabzie"),
						CreateDeclensionForm(Accusative, Singular, ["kabze", "kabzę"]),
						CreateDeclensionForm(Instrumental, Singular, ["kabzą", "kabzom"]),
						CreateDeclensionForm(Locative, Singular, ["kabzi", "kabzie"]),
						CreateDeclensionForm(Vocative, Singular, "kabzo"),
						CreateDeclensionForm(Nominative, Plural, "kabzy"),
						CreateDeclensionForm(Genitive, Plural, "kabz"),
						CreateDeclensionForm(Dative, Plural, "kabzom"),
						CreateDeclensionForm(Accusative, Plural, "kabzy"),
						CreateDeclensionForm(Instrumental, Plural, "kabzami"),
						CreateDeclensionForm(Locative, Plural, "kabzach"),
						CreateDeclensionForm(Vocative, Plural, "kabzy"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithPotentialWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("Polska")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "Polska"),
						CreateDeclensionForm(Genitive, Singular, "Polski"),
						CreateDeclensionForm(Dative, Singular, "Polsce"),
						CreateDeclensionForm(Accusative, Singular, "Polskę"),
						CreateDeclensionForm(Instrumental, Singular, "Polską"),
						CreateDeclensionForm(Locative, Singular, "Polsce"),
						CreateDeclensionForm(Vocative, Singular, "Polsko"),
						CreateDeclensionForm(Nominative, Plural, "Polski", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Genitive, Plural, "Polsk", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Dative, Plural, "Polskom", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Accusative, Plural, "Polski", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Instrumental, Plural, "Polskami", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Locative, Plural, "Polskach", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Vocative, Plural, "Polski", flags: DeclensionFormFlags.Potential),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithMultiplePotentialWordFormsWithinOneTemplate_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("biologia")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "biologia"),
						CreateDeclensionForm(Genitive, Singular, "biologii"),
						CreateDeclensionForm(Dative, Singular, "biologii"),
						CreateDeclensionForm(Accusative, Singular, "biologię"),
						CreateDeclensionForm(Instrumental, Singular, "biologią"),
						CreateDeclensionForm(Locative, Singular, "biologii"),
						CreateDeclensionForm(Vocative, Singular, "biologio"),
						CreateDeclensionForm(Nominative, Plural, "biologie", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Genitive, Plural, CreateFormValue("biologii", DeclensionFormFlags.Potential), CreateFormValue("biologij", DeclensionFormFlags.Potential | DeclensionFormFlags.Obsolete)),
						CreateDeclensionForm(Dative, Plural, "biologiom", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Accusative, Plural, "biologie", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Instrumental, Plural, "biologiami", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Locative, Plural, "biologiach", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Vocative, Plural, "biologie", flags: DeclensionFormFlags.Potential),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithMultiplePotentialWordFormsWithinMultipleTemplates_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("Fryzja")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "Fryzja"),
						CreateDeclensionForm(Genitive, Singular, "Fryzji"),
						CreateDeclensionForm(Dative, Singular, "Fryzji"),
						CreateDeclensionForm(Accusative, Singular, "Fryzję"),
						CreateDeclensionForm(Instrumental, Singular, "Fryzją"),
						CreateDeclensionForm(Locative, Singular, "Fryzji"),
						CreateDeclensionForm(Vocative, Singular, "Fryzjo"),
						CreateDeclensionForm(Nominative, Plural, "Fryzje", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Genitive, Plural, CreateFormValue("Fryzji", DeclensionFormFlags.Potential), CreateFormValue("Fryzyj", DeclensionFormFlags.Potential | DeclensionFormFlags.Obsolete)),
						CreateDeclensionForm(Dative, Plural, "Fryzjom", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Accusative, Plural, "Fryzje", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Instrumental, Plural, "Fryzjami", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Locative, Plural, "Fryzjach", flags: DeclensionFormFlags.Potential),
						CreateDeclensionForm(Vocative, Plural, "Fryzje", flags: DeclensionFormFlags.Potential),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithObsoleteWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("dom")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "dom"),
						CreateDeclensionForm(Genitive, Singular, "domu"),
						CreateDeclensionForm(Dative, Singular, "domowi"),
						CreateDeclensionForm(Accusative, Singular, "dom"),
						CreateDeclensionForm(Instrumental, Singular, "domem"),
						CreateDeclensionForm(Locative, Singular, CreateFormValue("domu"), CreateFormValue("domie", DeclensionFormFlags.Obsolete)),
						CreateDeclensionForm(Vocative, Singular, CreateFormValue("domu"), CreateFormValue("domie", DeclensionFormFlags.Obsolete)),
						CreateDeclensionForm(Nominative, Plural, "domy"),
						CreateDeclensionForm(Genitive, Plural, "domów"),
						CreateDeclensionForm(Dative, Plural, "domom"),
						CreateDeclensionForm(Accusative, Plural, "domy"),
						CreateDeclensionForm(Instrumental, Plural, "domami"),
						CreateDeclensionForm(Locative, Plural, "domach"),
						CreateDeclensionForm(Vocative, Plural, "domy"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithOldWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("audycja")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "audycja"),
						CreateDeclensionForm(Genitive, Singular, "audycji"),
						CreateDeclensionForm(Dative, Singular, "audycji"),
						CreateDeclensionForm(Accusative, Singular, "audycję"),
						CreateDeclensionForm(Instrumental, Singular, "audycją"),
						CreateDeclensionForm(Locative, Singular, "audycji"),
						CreateDeclensionForm(Vocative, Singular, "audycjo"),
						CreateDeclensionForm(Nominative, Plural, "audycje"),
						CreateDeclensionForm(Genitive, Plural, CreateFormValue("audycji"), CreateFormValue("audycyj", DeclensionFormFlags.Old)),
						CreateDeclensionForm(Dative, Plural, "audycjom"),
						CreateDeclensionForm(Accusative, Plural, "audycje"),
						CreateDeclensionForm(Instrumental, Plural, "audycjami"),
						CreateDeclensionForm(Locative, Plural, "audycjach"),
						CreateDeclensionForm(Vocative, Plural, "audycje"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithRareWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("dziewierz")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "dziewierz"),
						CreateDeclensionForm(Genitive, Singular, "dziewierza"),
						CreateDeclensionForm(Dative, Singular, "dziewierzowi"),
						CreateDeclensionForm(Accusative, Singular, "dziewierza"),
						CreateDeclensionForm(Instrumental, Singular, "dziewierzem"),
						CreateDeclensionForm(Locative, Singular, "dziewierzu"),
						CreateDeclensionForm(Vocative, Singular, "dziewierzu"),
						CreateDeclensionForm(Nominative, Plural, "dziewierze"),
						CreateDeclensionForm(Genitive, Plural, CreateFormValue("dziewierzy"), CreateFormValue("dziewierzów", DeclensionFormFlags.Rare)),
						CreateDeclensionForm(Dative, Plural, "dziewierzom"),
						CreateDeclensionForm(Accusative, Plural, CreateFormValue("dziewierzy"), CreateFormValue("dziewierzów", DeclensionFormFlags.Rare)),
						CreateDeclensionForm(Instrumental, Plural, "dziewierzami"),
						CreateDeclensionForm(Locative, Plural, "dziewierzach"),
						CreateDeclensionForm(Vocative, Plural, "dziewierze"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithColloquialWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("pomidor")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "pomidor"),
						CreateDeclensionForm(Genitive, Singular, "pomidora"),
						CreateDeclensionForm(Dative, Singular, "pomidorowi"),
						CreateDeclensionForm(Accusative, Singular, CreateFormValue("pomidor"), CreateFormValue("pomidora", DeclensionFormFlags.Colloquial)),
						CreateDeclensionForm(Instrumental, Singular, "pomidorem"),
						CreateDeclensionForm(Locative, Singular, "pomidorze"),
						CreateDeclensionForm(Vocative, Singular, "pomidorze"),
						CreateDeclensionForm(Nominative, Plural, "pomidory"),
						CreateDeclensionForm(Genitive, Plural, "pomidorów"),
						CreateDeclensionForm(Dative, Plural, "pomidorom"),
						CreateDeclensionForm(Accusative, Plural, "pomidory"),
						CreateDeclensionForm(Instrumental, Plural, "pomidorami"),
						CreateDeclensionForm(Locative, Plural, "pomidorach"),
						CreateDeclensionForm(Vocative, Plural, "pomidory"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithSublimeWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("bankowiec")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "bankowiec"),
						CreateDeclensionForm(Genitive, Singular, "bankowca"),
						CreateDeclensionForm(Dative, Singular, "bankowcowi"),
						CreateDeclensionForm(Accusative, Singular, "bankowca"),
						CreateDeclensionForm(Instrumental, Singular, "bankowcem"),
						CreateDeclensionForm(Locative, Singular, "bankowcu"),
						CreateDeclensionForm(Vocative, Singular, CreateFormValue("bankowcu"), CreateFormValue("bankowcze", DeclensionFormFlags.Sublime)),
						CreateDeclensionForm(Nominative, Plural, "bankowcy"),
						CreateDeclensionForm(Genitive, Plural, "bankowców"),
						CreateDeclensionForm(Dative, Plural, "bankowcom"),
						CreateDeclensionForm(Accusative, Plural, "bankowców"),
						CreateDeclensionForm(Instrumental, Plural, "bankowcami"),
						CreateDeclensionForm(Locative, Plural, "bankowcach"),
						CreateDeclensionForm(Vocative, Plural, "bankowcy"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithCharacteristicWordForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("prasowalnia")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "prasowalnia"),
						CreateDeclensionForm(Genitive, Singular, "prasowalni"),
						CreateDeclensionForm(Dative, Singular, "prasowalni"),
						CreateDeclensionForm(Accusative, Singular, "prasowalnię"),
						CreateDeclensionForm(Instrumental, Singular, "prasowalnią"),
						CreateDeclensionForm(Locative, Singular, "prasowalni"),
						CreateDeclensionForm(Vocative, Singular, "prasowalnio"),
						CreateDeclensionForm(Nominative, Plural, "prasowalnie"),
						CreateDeclensionForm(Genitive, Plural, CreateFormValue("prasowalni"), CreateFormValue("prasowalń", DeclensionFormFlags.Characteristic)),
						CreateDeclensionForm(Dative, Plural, "prasowalniom"),
						CreateDeclensionForm(Accusative, Plural, "prasowalnie"),
						CreateDeclensionForm(Instrumental, Plural, "prasowalniami"),
						CreateDeclensionForm(Locative, Plural, "prasowalniach"),
						CreateDeclensionForm(Vocative, Plural, "prasowalnie"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithoutPluralForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("zenit")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "zenit"),
						CreateDeclensionForm(Genitive, Singular, "zenitu"),
						CreateDeclensionForm(Dative, Singular, "zenitowi"),
						CreateDeclensionForm(Accusative, Singular, "zenit"),
						CreateDeclensionForm(Instrumental, Singular, "zenitem"),
						CreateDeclensionForm(Locative, Singular, "zenicie"),
						CreateDeclensionForm(Vocative, Singular, "zenicie"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithDepreciativeForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("bohater")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "bohater"),
						CreateDeclensionForm(Genitive, Singular, "bohatera"),
						CreateDeclensionForm(Dative, Singular, "bohaterowi"),
						CreateDeclensionForm(Accusative, Singular, "bohatera"),
						CreateDeclensionForm(Instrumental, Singular, "bohaterem"),
						CreateDeclensionForm(Locative, Singular, "bohaterze"),
						CreateDeclensionForm(Vocative, Singular, "bohaterze"),
						CreateDeclensionForm(Nominative, Plural, ["bohaterowie", "bohaterzy"]),
						CreateDeclensionForm(Genitive, Plural, "bohaterów"),
						CreateDeclensionForm(Dative, Plural, "bohaterom"),
						CreateDeclensionForm(Accusative, Plural, "bohaterów"),
						CreateDeclensionForm(Instrumental, Plural, "bohaterami"),
						CreateDeclensionForm(Locative, Plural, "bohaterach"),
						CreateDeclensionForm(Vocative, Plural, ["bohaterowie", "bohaterzy"]),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithNonDepreciativeForms_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("wnuk")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "wnuk"),
						CreateDeclensionForm(Genitive, Singular, "wnuka"),
						CreateDeclensionForm(Dative, Singular, "wnukowi"),
						CreateDeclensionForm(Accusative, Singular, "wnuka"),
						CreateDeclensionForm(Instrumental, Singular, "wnukiem"),
						CreateDeclensionForm(Locative, Singular, "wnuku"),
						CreateDeclensionForm(Vocative, Singular, "wnuku"),
						CreateDeclensionForm(Nominative, Plural, "wnuki"),
						CreateDeclensionForm(Genitive, Plural, "wnuków"),
						CreateDeclensionForm(Dative, Plural, "wnukom"),
						CreateDeclensionForm(Accusative, Plural, "wnuków"),
						CreateDeclensionForm(Instrumental, Plural, "wnukami"),
						CreateDeclensionForm(Locative, Plural, "wnukach"),
						CreateDeclensionForm(Vocative, Plural, "wnuki"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounWithFlagAfterForm_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("Iberia")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "Iberia"),
						CreateDeclensionForm(Genitive, Singular, "Iberii"),
						CreateDeclensionForm(Dative, Singular, "Iberii"),
						CreateDeclensionForm(Accusative, Singular, "Iberię"),
						CreateDeclensionForm(Instrumental, Singular, "Iberią"),
						CreateDeclensionForm(Locative, Singular, "Iberii"),
						CreateDeclensionForm(Vocative, Singular, "Iberio"),
						CreateDeclensionForm(Nominative, Plural, "Iberie"),
						CreateDeclensionForm(Genitive, Plural, CreateFormValue("Iberii"), CreateFormValue("Iberyj", DeclensionFormFlags.Obsolete)),
						CreateDeclensionForm(Dative, Plural, "Iberiom"),
						CreateDeclensionForm(Accusative, Plural, "Iberie"),
						CreateDeclensionForm(Instrumental, Plural, "Iberiami"),
						CreateDeclensionForm(Locative, Plural, "Iberiach"),
						CreateDeclensionForm(Vocative, Plural, "Iberie"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForNounPhrase_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("system operacyjny")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "system operacyjny"),
						CreateDeclensionForm(Genitive, Singular, "systemu operacyjnego"),
						CreateDeclensionForm(Dative, Singular, "systemowi operacyjnemu"),
						CreateDeclensionForm(Accusative, Singular, "system operacyjny"),
						CreateDeclensionForm(Instrumental, Singular, "systemem operacyjnym"),
						CreateDeclensionForm(Locative, Singular, "systemie operacyjnym"),
						CreateDeclensionForm(Vocative, Singular, "systemie operacyjny"),
						CreateDeclensionForm(Nominative, Plural, "systemy operacyjne"),
						CreateDeclensionForm(Genitive, Plural, "systemów operacyjnych"),
						CreateDeclensionForm(Dative, Plural, "systemom operacyjnym"),
						CreateDeclensionForm(Accusative, Plural, "systemy operacyjne"),
						CreateDeclensionForm(Instrumental, Plural, "systemami operacyjnymi"),
						CreateDeclensionForm(Locative, Plural, "systemach operacyjnych"),
						CreateDeclensionForm(Vocative, Plural, "systemy operacyjne"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForMultipleDefinitionsDelimitedWithLub_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("chlorek")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "chlorek"),
						CreateDeclensionForm(Genitive, Singular, "chlorku"),
						CreateDeclensionForm(Dative, Singular, "chlorkowi"),
						CreateDeclensionForm(Accusative, Singular, "chlorek"),
						CreateDeclensionForm(Instrumental, Singular, "chlorkiem"),
						CreateDeclensionForm(Locative, Singular, "chlorku"),
						CreateDeclensionForm(Vocative, Singular, "chlorku"),
					],
				},

				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "chlorek"),
						CreateDeclensionForm(Genitive, Singular, "chlorku"),
						CreateDeclensionForm(Dative, Singular, "chlorkowi"),
						CreateDeclensionForm(Accusative, Singular, "chlorek"),
						CreateDeclensionForm(Instrumental, Singular, "chlorkiem"),
						CreateDeclensionForm(Locative, Singular, "chlorku"),
						CreateDeclensionForm(Vocative, Singular, "chlorku"),
						CreateDeclensionForm(Nominative, Plural, "chlorki"),
						CreateDeclensionForm(Genitive, Plural, "chlorków"),
						CreateDeclensionForm(Dative, Plural, "chlorkom"),
						CreateDeclensionForm(Accusative, Plural, "chlorki"),
						CreateDeclensionForm(Instrumental, Plural, "chlorkami"),
						CreateDeclensionForm(Locative, Plural, "chlorkach"),
						CreateDeclensionForm(Vocative, Plural, "chlorki"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForIndeclinableNoun_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("via")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "via"),
						CreateDeclensionForm(Genitive, Singular, "via"),
						CreateDeclensionForm(Dative, Singular, "via"),
						CreateDeclensionForm(Accusative, Singular, "via"),
						CreateDeclensionForm(Instrumental, Singular, "via"),
						CreateDeclensionForm(Locative, Singular, "via"),
						CreateDeclensionForm(Vocative, Singular, "via"),
						CreateDeclensionForm(Nominative, Plural, "via"),
						CreateDeclensionForm(Genitive, Plural, "via"),
						CreateDeclensionForm(Dative, Plural, "via"),
						CreateDeclensionForm(Accusative, Plural, "via"),
						CreateDeclensionForm(Instrumental, Plural, "via"),
						CreateDeclensionForm(Locative, Plural, "via"),
						CreateDeclensionForm(Vocative, Plural, "via"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseWordDefinitions_ForMultipleDefinitionsIncludingInDeclinableDelimitedWithLub_ReturnsCorrectWordDefinition()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var wordDefinitions = target.ParseWordDefinitions(0, CreateLanguageWordData("procent")).ToList();

			// Assert

			var expectedWordDefinitions = new[]
			{
				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "procent"),
						CreateDeclensionForm(Genitive, Singular, ["procentu", "procenta"]),
						CreateDeclensionForm(Dative, Singular, "procentowi"),
						CreateDeclensionForm(Accusative, Singular, "procent"),
						CreateDeclensionForm(Instrumental, Singular, "procentem"),
						CreateDeclensionForm(Locative, Singular, "procencie"),
						CreateDeclensionForm(Vocative, Singular, "procencie"),
						CreateDeclensionForm(Nominative, Plural, "procent"),
						CreateDeclensionForm(Genitive, Plural, "procent"),
						CreateDeclensionForm(Dative, Plural, "procentom"),
						CreateDeclensionForm(Accusative, Plural, "procent"),
						CreateDeclensionForm(Instrumental, Plural, "procentami"),
						CreateDeclensionForm(Locative, Plural, "procentach"),
						CreateDeclensionForm(Vocative, Plural, "procent"),
					],
				},

				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "procent"),
						CreateDeclensionForm(Genitive, Singular, "procent"),
						CreateDeclensionForm(Dative, Singular, "procent"),
						CreateDeclensionForm(Accusative, Singular, "procent"),
						CreateDeclensionForm(Instrumental, Singular, "procent"),
						CreateDeclensionForm(Locative, Singular, "procent"),
						CreateDeclensionForm(Vocative, Singular, "procent"),
						CreateDeclensionForm(Nominative, Plural, "procent"),
						CreateDeclensionForm(Genitive, Plural, "procent"),
						CreateDeclensionForm(Dative, Plural, "procent"),
						CreateDeclensionForm(Accusative, Plural, "procent"),
						CreateDeclensionForm(Instrumental, Plural, "procent"),
						CreateDeclensionForm(Locative, Plural, "procent"),
						CreateDeclensionForm(Vocative, Plural, "procent"),
					],
				},

				new NounDefinition
				{
					Forms =
					[
						CreateDeclensionForm(Nominative, Singular, "procent"),
						CreateDeclensionForm(Genitive, Singular, "procentu"),
						CreateDeclensionForm(Dative, Singular, "procentowi"),
						CreateDeclensionForm(Accusative, Singular, "procent"),
						CreateDeclensionForm(Instrumental, Singular, "procentem"),
						CreateDeclensionForm(Locative, Singular, "procencie"),
						CreateDeclensionForm(Vocative, Singular, "procencie"),
						CreateDeclensionForm(Nominative, Plural, "procenty"),
						CreateDeclensionForm(Genitive, Plural, "procentów"),
						CreateDeclensionForm(Dative, Plural, "procentom"),
						CreateDeclensionForm(Accusative, Plural, "procenty"),
						CreateDeclensionForm(Instrumental, Plural, "procentami"),
						CreateDeclensionForm(Locative, Plural, "procentach"),
						CreateDeclensionForm(Vocative, Plural, "procenty"),
					],
				},
			};

			wordDefinitions.Should().BeEquivalentTo(expectedWordDefinitions, x => x.WithStrictOrdering());
		}

		private static LanguageWordData CreateLanguageWordData(string word)
		{
			return new LanguageWordData
			{
				Word = word,
				Wikitext = LoadWikitext(Path.ChangeExtension(word, ".txt")),
			};
		}

		private static string LoadWikitext(string fileName)
		{
			var filePath = Path.Combine("test-data", fileName);

			return File.ReadAllText(filePath);
		}

		private static WiktionaryPageParser CreateTestTarget()
		{
			return new WiktionaryPageParser(new WikitextParser());
		}
	}
}
