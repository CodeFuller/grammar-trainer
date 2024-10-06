using System.Collections.Generic;
using IndexBuilder.Model;

namespace IndexBuilder.Data.Output
{
	internal class NounDefinitionData
	{
		public string Word { get; init; }

		public IReadOnlyCollection<NounDefinition> Definitions { get; init; }
	}
}
