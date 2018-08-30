using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Core.Utilities
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
			this.Destination = destinationRectangle;
			this.Source = sourceRectangle;
			this.Origin = origin;
			this.Rotation = rotation;
			this.Effect = effect;
			this.Color = color;
			this.IsDone = isDone;
			this.Caller = caller;
		}
	}
}