using System;

namespace Singularity.Core
{
	[Flags]
	public enum ChildProperties
	{
		Nothing = 0b00000000,
		None = 0b00000000,
		Translation = 0b00000001,
		Rotation = 0b00000010,
		Scale = 0b00000100,
		TranslationRotation = Translation | Rotation,
		DrawMode = 0b01000000,
		KeepPositon = 0b10000000,
		AllTransform = Translation | Rotation | Scale,
		All = AllTransform | DrawMode
	}
}