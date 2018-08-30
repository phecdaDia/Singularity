namespace Singularity.Core.Utilities
{
	public enum RotationMode
	{
		// always 2 bit specify an axis.
		//
		// None = 00
		// X    = 01
		// Y    = 10
		// Z    = 11
		// rotation shall be executed like this:
		// 0b00XXYYZZ => Z rotation, then Y rotation then X rotation. Going from right to left

		None = 0b00,
		X    = 0b01,
		Y    = 0b10,
		Z    = 0b11,

		XYZ = X << 0 | Y << 2 | Z << 4,
		XZY = X << 0 | Y << 4 | Z << 2,

		YXZ = X << 2 | Y << 0 | Z << 4,
		YZX = X << 4 | Y << 0 | Z << 2,

		ZYX  = X << 4 | Y << 2 | Z << 0,
		ZXY  = X << 2 | Y << 4 | Z << 0,

	}
}
