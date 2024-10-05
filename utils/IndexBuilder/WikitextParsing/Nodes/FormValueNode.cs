using System;
using System.Collections.Generic;
using System.Linq;
using IndexBuilder.Model;

namespace IndexBuilder.WikitextParsing.Nodes
{
	internal class FormValueNode
	{
		private static readonly Dictionary<string, DeclensionFormFlags> SupportedFlags = new()
		{
			{ "przest", DeclensionFormFlags.Obsolete },
			{ "daw", DeclensionFormFlags.Old },
			{ "rzad", DeclensionFormFlags.Rare },
			{ "pot", DeclensionFormFlags.Colloquial },
			{ "podn", DeclensionFormFlags.Sublime },
			{ "char", DeclensionFormFlags.Characteristic },
			{ "m", DeclensionFormFlags.GenderMasculine },
			{ "mos", DeclensionFormFlags.GenderMasculinePersonal },
			{ "mzw", DeclensionFormFlags.GenderMasculineAnimal },
			{ "mrz", DeclensionFormFlags.GenderMasculineThing },
			{ "nmos", DeclensionFormFlags.GenderNonMasculinePersonal },
			{ "fakt", DeclensionFormFlags.RequiresVerification },
			{ "książk", DeclensionFormFlags.Bookish },
			{ "reg", DeclensionFormFlags.Regional },
			{ "poet", DeclensionFormFlags.Poetic },
			{ "neutr", DeclensionFormFlags.Neutral },
			{ "ndepr", DeclensionFormFlags.NonDepreciative },
			{ "pieszcz", DeclensionFormFlags.Tender },
			{ "popr", DeclensionFormFlags.Correct },
			{ "przen", DeclensionFormFlags.Figuratively },
			{ "archit", DeclensionFormFlags.Architectural },
			{ "także", DeclensionFormFlags.Also },
			{ "specific-preposition", DeclensionFormFlags.SpecificPreposition },
		};

		public string Word { get; }

		public DeclensionFormFlags Flags { get; }

		public FormValueNode(string word)
		{
			Word = word;
			Flags = DeclensionFormFlags.None;
		}

		public FormValueNode(FormValueNode formValueNode, DeclensionFormFlags flags)
		{
			Word = formValueNode.Word;
			Flags = formValueNode.Flags | flags;
		}

		public FormValueNode WithFlag(DeclensionFormFlags flag)
		{
			return new FormValueNode(this, flag);
		}

		public FormValueNode WithFlags(IEnumerable<string> flags)
		{
			return WithFlag(flags.Aggregate(DeclensionFormFlags.None, (x, y) => x | ConvertFlagValue(y)));
		}

		private static DeclensionFormFlags ConvertFlagValue(string flagValue)
		{
			if (SupportedFlags.TryGetValue(flagValue, out var flag))
			{
				return flag;
			}

			throw new NotSupportedException($"The value of declension form flag is not supported: '{flagValue}'");
		}

		public static FormValuesNode operator +(FormValueNode a, FormValuesNode b)
		{
			return new FormValuesNode(Enumerable.Repeat(a, 1).Concat(b.FormValues));
		}
	}
}
