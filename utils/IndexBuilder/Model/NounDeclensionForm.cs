using System.Collections.Generic;

namespace IndexBuilder.Model
{
	internal class NounDeclensionForm
	{
		public GrammaticalCase Case { get; init; }

		public GrammaticalNumber Number { get; init; }

		public IReadOnlyCollection<NounDeclensionFormValue> Forms { get; init; }
	}
}
