using System;

namespace IndexBuilder.Model
{
	[Flags]
	internal enum DeclensionFormFlags
	{
		None = 0x000000,
		Potential = 0x000001,
		Obsolete = 0x000002,
		Old = 0x000004,
		Rare = 0x000008,
		Colloquial = 0x000010,
		Sublime = 0x000020,
		Characteristic = 0x000040,
		GenderMasculine = 0x000080,
		GenderMasculinePersonal = 0x000100,
		GenderMasculineAnimal = 0x000200,
		GenderMasculineThing = 0x000400,
		GenderNonMasculinePersonal = 0x000800,
		RequiresVerification = 0x001000,
		Bookish = 0x002000,
		Poetic = 0x004000,
		Regional = 0x008000,
		Neutral = 0x010000,
		NonDepreciative = 0x020000,
		Tender = 0x040000,
		Correct = 0x080000,
		Figuratively = 0x100000,
		Architectural = 0x200000,
		Also = 0x400000,
		SpecificPreposition = 0x800000,
	}
}
