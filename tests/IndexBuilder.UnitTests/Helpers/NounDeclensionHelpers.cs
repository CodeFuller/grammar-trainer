using System.Linq;
using IndexBuilder.Model;

namespace IndexBuilder.UnitTests.Helpers
{
	internal static class NounDeclensionHelpers
	{
		public static NounDeclensionForm CreateDeclensionForm(GrammaticalCase grammaticalCase, GrammaticalNumber grammaticalNumber, string form, DeclensionFormFlags flags = DeclensionFormFlags.None)
		{
			return CreateDeclensionForm(grammaticalCase, grammaticalNumber, [form], flags);
		}

		public static NounDeclensionForm CreateDeclensionForm(GrammaticalCase grammaticalCase, GrammaticalNumber grammaticalNumber, string[] forms, DeclensionFormFlags flags = DeclensionFormFlags.None)
		{
			return CreateDeclensionForm(grammaticalCase, grammaticalNumber, forms.Select(x => new NounDeclensionFormValue { FormValue = x, Flags = flags }).ToArray());
		}

		public static NounDeclensionForm CreateDeclensionForm(GrammaticalCase grammaticalCase, GrammaticalNumber grammaticalNumber, params NounDeclensionFormValue[] forms)
		{
			return new NounDeclensionForm
			{
				Case = grammaticalCase,
				Number = grammaticalNumber,
				FormValues = forms,
			};
		}

		public static NounDeclensionFormValue CreateFormValue(string form, DeclensionFormFlags flags = DeclensionFormFlags.None)
		{
			return new NounDeclensionFormValue
			{
				FormValue = form,
				Flags = flags,
			};
		}
	}
}
