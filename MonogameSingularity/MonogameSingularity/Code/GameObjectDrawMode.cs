using System;

namespace Singularity
{
	[Flags]
	public enum GameObjectDrawMode
	{
		Nothing     = 0b00000000,	// Object won't be drawn
		Model       = 0b00000001,	// Only the 3d part will be drawn
		SpriteBatch = 0b00000010,	// Only 2d will be drawn
		All         = 0b11111111	// Everything will be drawn
		
	}
}