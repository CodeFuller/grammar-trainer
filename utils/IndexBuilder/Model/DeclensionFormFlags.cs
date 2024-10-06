using System;

namespace IndexBuilder.Model
{
	[Flags]
	internal enum DeclensionFormFlags
	{
		None = 0x0000000,
		Potential = 0x0000001,
		Obsolete = 0x0000002,
		Old = 0x0000004,
		Rare = 0x0000008,
		Colloquial = 0x0000010,
		Sublime = 0x0000020,
		Characteristic = 0x0000040,
		GenderMasculine = 0x0000080,
		GenderMasculinePersonal = 0x0000100,
		GenderMasculineAnimal = 0x0000200,
		GenderMasculineThing = 0x0000400,
		GenderNonMasculinePersonal = 0x0000800,
		RequiresVerification = 0x0001000,
		Bookish = 0x0002000,
		Poetic = 0x0004000,
		Regional = 0x0008000,
		Neutral = 0x0010000,
		NonDepreciative = 0x0020000,
		Tender = 0x0040000,
		Correct = 0x0080000,
		Figuratively = 0x0100000,
		Architectural = 0x0200000,
		Also = 0x0400000,
		Ironical = 0x0800000,
		SpecificPreposition = 0x1000000,
	}
}
