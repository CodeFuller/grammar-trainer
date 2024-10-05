using System;
using System.Linq;
using FluentAssertions;
using IndexBuilder.Model;
using IndexBuilder.WikitextParsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using static IndexBuilder.UnitTests.Helpers.NounDeclensionHelpers;

namespace IndexBuilder.UnitTests.WikitextParsing
{
	[TestClass]
	public class PidginFormValuesParserTests
	{
		[TestMethod]
		public void ParseFormValues_ForSingleFormValue_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("hello a'b’c.!d-e").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("hello a'b’c.!d-e"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[DataTestMethod]
		[DataRow("cat / dog")]
		[DataRow("cat/dog")]
		[DataRow("cat<br />dog")]
		[DataRow("cat<br/>dog")]
		[DataRow("cat<br>dog")]
		[DataRow("cat, dog")]
		[DataRow("cat ''lub'' dog")]
		public void ParseFormValues_ForMultipleFormValues_ReturnsCorrectFormValues(string wikitext)
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues(wikitext).ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
				CreateFormValue("dog"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForFormValueAsWikilink_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("[[cat]]").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForSingleFormValueWithPrefixFlag_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("{{przest}} cat").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Obsolete),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForSingleFormValueWithSuffixFlag_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat {{przest}}").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Obsolete),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[DataTestMethod]
		[DataRow("{{przest}} {{rzad}} cat")]
		[DataRow("cat {{przest}} {{rzad}}")]
		[DataRow("{{przest}} cat {{rzad}}")]
		public void ParseFormValues_ForSingleFormValueWithMultipleFlags_ReturnsCorrectFormValues(string wikitext)
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues(wikitext).ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Obsolete | DeclensionFormFlags.Rare),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForSingleFormValueWithNonPairedRefTag_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat<ref name=\"test\" />").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForSingleFormValueWithPairedRefTag_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat<ref>test</ref>").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForSingleFormValueWithMultipleRefTags_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat<ref>test</ref><ref name=\"test\" />").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForReferenceAfterFlag_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat {{podn}}<ref>test</ref>").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Sublime),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForWikitextWithWhitespaces_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat ").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForWikitextWithSinglePotentialFormsTemplateWithSingleFormValue_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("{{potencjalnie|cat}}").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Potential),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForWikitextWithSinglePotentialFormsTemplateWithMultipleFormValues_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("{{potencjalnie|cat / {{przest}} dog}}").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Potential),
				CreateFormValue("dog", DeclensionFormFlags.Potential | DeclensionFormFlags.Obsolete),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForWikitextWithMultiplePotentialFormTemplates_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("{{potencjalnie|cat}} / {{potencjalnie|{{przest}} dog}}").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Potential),
				CreateFormValue("dog", DeclensionFormFlags.Potential | DeclensionFormFlags.Obsolete),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForWikitextWithPotentialTemplateAndNonPotentialFormValues_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("cat / {{potencjalnie|dog}}").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
				CreateFormValue("dog", DeclensionFormFlags.Potential),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ParseFormValues_ForWikitextWithPotentialTemplateAndReference_ReturnsCorrectFormValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues("{{potencjalnie|cat / dog}}<ref />").ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat", DeclensionFormFlags.Potential),
				CreateFormValue("dog", DeclensionFormFlags.Potential),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[DataTestMethod]
		[DataRow("cat / {{przest}} ''(tylko po „ku”)'' dog")]
		[DataRow("cat / {{przest}} ''tylko po „ku”'' dog")]
		[DataRow("cat / {{przest}} ''tylko po \"ku\"'' dog")]
		[DataRow("cat / {{przest}} ''tylko po „[[ku]]”'' dog")]
		public void ParseFormValues_ForWikitextWithFormsForSpecificPreposition_ReturnsCorrectFormValues(string wikitext)
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var formValues = target.ParseFormValues(wikitext).ToList();

			// Assert

			var expectedFormValues = new[]
			{
				CreateFormValue("cat"),
				CreateFormValue("dog", DeclensionFormFlags.Obsolete | DeclensionFormFlags.SpecificPreposition),
			};

			formValues.Should().BeEquivalentTo(expectedFormValues, x => x.WithStrictOrdering());
		}

		[DataRow("")]
		[DataRow("abc?")]
		[DataRow("{{przest}}")]
		[DataTestMethod]
		public void ParseFormValues_ForIncorrectWikitext_Throws(string wikitext)
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PidginFormValuesParser>();

			// Act

			var call = () => target.ParseFormValues(wikitext).ToList();

			// Assert

			call.Should().Throw<InvalidOperationException>();
		}
	}
}
