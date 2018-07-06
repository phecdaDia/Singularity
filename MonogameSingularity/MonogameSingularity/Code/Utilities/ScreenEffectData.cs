using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Utilities
{
	public struct ScreenEffectData
	{
		public Rectangle Destination;
		public Rectangle Source;
		public Vector2 Origin;
		public float? Rotation;
		public SpriteEffects? Effect;
		public Color? Color;
		public bool IsDone;
		public object Caller;

		public ScreenEffectData(Rectangle destinationRectangle, Rectangle sourceRectangle, Vector2 origin, float rotation,
			SpriteEffects effect, Color color, bool isDone, object caller)
		{
			Destination = destinationRectangle;
			Source = sourceRectangle;
			Origin = origin;
			Rotation = rotation;
			Effect = effect;
			Color = color;
			IsDone = isDone;
			Caller = caller;
		}
	}
}