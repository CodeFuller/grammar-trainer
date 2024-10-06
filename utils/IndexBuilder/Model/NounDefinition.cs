using System.Collections.Generic;

namespace IndexBuilder.Model
{
	internal class NounDefinition : WordDefinition
	{
		public IReadOnlyCollection<NounDeclensionForm> Forms { get; init; }
	}
}
