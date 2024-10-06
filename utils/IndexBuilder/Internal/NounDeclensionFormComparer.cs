using System.Collections.Generic;
using IndexBuilder.Model;

namespace IndexBuilder.Internal
{
	internal class NounDeclensionFormComparer : IComparer<NounDeclensionForm>
	{
		public int Compare(NounDeclensionForm x, NounDeclensionForm y)
		{
			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (ReferenceEquals(null, y))
			{
				return 1;
			}

			if (ReferenceEquals(null, x))
			{
				return -1;
			}

			var numberComparison = x.Number.CompareTo(y.Number);
			if (numberComparison != 0)
			{
				return numberComparison;
			}

			return x.Case.CompareTo(y.Case);
		}
	}
}
