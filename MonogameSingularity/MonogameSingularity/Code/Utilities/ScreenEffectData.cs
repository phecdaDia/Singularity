using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code.Utilities
{
	public struct ScreenEffectData
	{
		public Rectangle Desitnation;
		public Rectangle Source;
		public Vector2 Origin;
		public float Rotation;
		public SpriteEffects Effect;
		public Color Color;

		public ScreenEffectData(Rectangle destinationRectangle, Rectangle sourceRectangle, Vector2 origin, float rotation, SpriteEffects effect, Color color)
		{
			Desitnation = destinationRectangle;
			Source = sourceRectangle;
			Origin = origin;
			Rotation = rotation;
			Effect = effect;
			Color = color;
		}

		public ScreenEffectData(Vector2 position, Vector2 size, SpriteEffects effect, Color color, Vector2 origin,
			float rotation = 0) : this(new Rectangle(position.ToPoint(), size.ToPoint()),
			new Rectangle(Vector2.Zero.ToPoint(), size.ToPoint()), origin, rotation, effect, color)
		{ }
	}
}
