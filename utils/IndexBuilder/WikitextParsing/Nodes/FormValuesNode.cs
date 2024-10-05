using System.Collections.Generic;
using System.Linq;
using IndexBuilder.Model;

namespace IndexBuilder.WikitextParsing.Nodes
{
	internal class FormValuesNode
	{
		public IReadOnlyCollection<FormValueNode> FormValues { get; }

		public FormValuesNode(FormValueNode formValueNode)
		{
			FormValues = [formValueNode];
		}

		public FormValuesNode(IEnumerable<FormValueNode> formValueNodes)
		{
			FormValues = formValueNodes.ToList();
		}

		public FormValuesNode WithFlag(DeclensionFormFlags flag)
		{
			return new FormValuesNode(FormValues.Select(x => x.WithFlag(flag)));
		}
	}
}
