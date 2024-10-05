using CommandLine;

namespace IndexBuilder
{
	internal sealed class CommandLineOptions
	{
		[Option('s', "source", Required = true, HelpText = "Path to wiktionary dump.")]
		public string SourcePath { get; set; }

		[Option('l', "language", Required = true, HelpText = "Target language.")]
		public string Language { get; set; }
	}
}
