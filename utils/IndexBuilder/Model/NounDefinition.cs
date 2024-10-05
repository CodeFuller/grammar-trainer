using System.Collections.Generic;
using IndexBuilder.Data;

namespace IndexBuilder.Model
{
	internal class NounDefinition : WordDefinition
	{
		public IReadOnlyCollection<NounDeclensionForm> Forms { get; init; }
	}
}
