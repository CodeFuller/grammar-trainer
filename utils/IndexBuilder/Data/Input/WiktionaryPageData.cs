using IndexBuilder.Internal;

namespace IndexBuilder.Data.Input
{
	internal sealed class WiktionaryPageData
	{
		public int Id { get; init; }

		public string Title { get; init; }

		public WiktionaryNamespace Namespace { get; init; }

		public bool IsRedirect { get; init; }

		public string Wikitext { get; init; }
	}
}
