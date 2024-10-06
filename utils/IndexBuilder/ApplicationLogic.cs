using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CodeFuller.Library.Bootstrap;
using CommandLine;
using IndexBuilder.Data.Input;
using IndexBuilder.Data.Output;
using IndexBuilder.Interfaces;
using IndexBuilder.Internal;
using Microsoft.Extensions.Logging;

namespace IndexBuilder
{
	internal sealed class ApplicationLogic : IApplicationLogic
	{
		private readonly IWiktionaryPageParser wiktionaryPageParser;

		private readonly IWordDefinitionsSerializer wordDefinitionsSerializer;

		private readonly ILogger<ApplicationLogic> logger;

		public ApplicationLogic(IWiktionaryPageParser wiktionaryPageParser, IWordDefinitionsSerializer wordDefinitionsSerializer, ILogger<ApplicationLogic> logger)
		{
			this.wiktionaryPageParser = wiktionaryPageParser ?? throw new ArgumentNullException(nameof(wiktionaryPageParser));
			this.wordDefinitionsSerializer = wordDefinitionsSerializer ?? throw new ArgumentNullException(nameof(wordDefinitionsSerializer));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);

			if (parseResult.Tag == ParserResultType.Parsed)
			{
				BuildLanguageIndex(parseResult.Value);
				return Task.FromResult(0);
			}

			return Task.FromResult(1);
		}

		private void BuildLanguageIndex(CommandLineOptions commandLineOptions)
		{
			var totalPagesCount = 0;
			var entryPagesCount = 0;
			var pageParsingFailuresCount = 0;
			var currentLanguageEntryPagesCount = 0;
			var formsParsingFailuresCount = 0;

			var wordDefinitionsData = new WordDefinitionsData();

			foreach (var page in GetWiktionaryPages(commandLineOptions.SourcePath))
			{
				++totalPagesCount;

				if (page.Namespace != WiktionaryNamespace.Entry || page.IsRedirect)
				{
					continue;
				}

				++entryPagesCount;

				var languageWords = wiktionaryPageParser.ParseLanguageWords(page.Wikitext).ToList();
				if (languageWords.Count == 0)
				{
					logger.LogWarning($"Failed to parse text from page {page.Id}: '{page.Title}'");
					++pageParsingFailuresCount;
					continue;
				}

				var wordInCurrentLanguage = languageWords.SingleOrDefault(x => x.Language == commandLineOptions.Language);
				if (wordInCurrentLanguage == null)
				{
					continue;
				}

				++currentLanguageEntryPagesCount;

				try
				{
					var wordDefinitions = wiktionaryPageParser.ParseWordDefinitions(page.Id, wordInCurrentLanguage).ToList();
					wordDefinitionsData.Add(wordInCurrentLanguage.Word, wordDefinitions);
				}
				catch (InvalidOperationException e)
				{
					logger.LogWarning($"Failed to parse word definitions from page {page.Id}: {e.Message}");
					++formsParsingFailuresCount;
				}
			}

			var statsBuilder = new StringBuilder();
			statsBuilder.AppendLine(CultureInfo.InvariantCulture, $"{"Total pages:",-40} {totalPagesCount,9:N0}");
			statsBuilder.AppendLine(CultureInfo.InvariantCulture, $"{"Entry pages:",-40} {entryPagesCount,9:N0}");
			statsBuilder.AppendLine(CultureInfo.InvariantCulture, $"{$"Entry pages in '{commandLineOptions.Language}' language:",-40} {currentLanguageEntryPagesCount,9:N0}");
			statsBuilder.AppendLine(CultureInfo.InvariantCulture, $"{"Page parsing failures:",-40} {pageParsingFailuresCount,9:N0}");
			statsBuilder.AppendLine(CultureInfo.InvariantCulture, $"{"Forms parsing failures:",-40} {formsParsingFailuresCount,9:N0}");

			logger.LogInformation($"Dump statistics:\n\n{statsBuilder}");

			var serializedData = wordDefinitionsSerializer.Serialize(commandLineOptions.Language, wordDefinitionsData);
			File.WriteAllText(commandLineOptions.TargetPath, serializedData);
		}

		private IEnumerable<WiktionaryPageData> GetWiktionaryPages(string wikiExportFilePath)
		{
			logger.LogInformation($"Loading dump from '{wikiExportFilePath}' ...");

			using var xmlFileStream = File.OpenRead(wikiExportFilePath);
			using var xmlReader = XmlReader.Create(xmlFileStream);

			xmlReader.MoveToContent();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (xmlReader.Name == "page")
						{
							var pageElement = (XElement)XNode.ReadFrom(xmlReader);

							var idElement = pageElement.XPathSelectElement("/*[local-name()='id']");
							if (idElement == null)
							{
								throw new InvalidOperationException("id element is missing in page element");
							}

							var titleElement = pageElement.XPathSelectElement("/*[local-name()='title']");
							if (titleElement == null)
							{
								throw new InvalidOperationException("title element is missing in page element");
							}

							var nsElement = pageElement.XPathSelectElement("/*[local-name()='ns']");
							if (nsElement == null)
							{
								throw new InvalidOperationException("ns element is missing in page element");
							}

							var textElement = pageElement.XPathSelectElement("/*[local-name()='revision']/*[local-name()='text']");
							if (textElement == null)
							{
								throw new InvalidOperationException("text element is missing in page element");
							}

							var redirectElement = pageElement.XPathSelectElement("/*[local-name()='redirect']");

							yield return new WiktionaryPageData
							{
								Id = Int32.Parse(idElement.Value, NumberStyles.None, CultureInfo.InvariantCulture),
								Title = titleElement.Value,
								Namespace = ConvertWiktionaryNamespace(nsElement.Value),
								IsRedirect = redirectElement != null,
								Wikitext = textElement.Value,
							};
						}

						break;
				}
			}
		}

		private static WiktionaryNamespace ConvertWiktionaryNamespace(string wiktionaryNamespace)
		{
			return wiktionaryNamespace == "0" ? WiktionaryNamespace.Entry : WiktionaryNamespace.Other;
		}
	}
}
