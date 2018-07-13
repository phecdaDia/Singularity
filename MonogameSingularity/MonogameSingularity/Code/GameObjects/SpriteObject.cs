using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.GameObjects
{
	public abstract class SpriteObject : GameObject
	{
		public SpriteObject()
		{
		}

		public SpriteObject(Texture2D texture) : this()
		{
			Texture = texture;
		}

		public SpriteObject(string textureKey) : this(SingularityGame.GetContentManager().Load<Texture2D>(textureKey))
		{
		}

		public Texture2D Texture { get; }
		public Color Color { get; private set; }
		public Vector2 Origin { get; private set; }
		public float Depth { get; private set; }

		public SpriteObject SetColor(Color color)
		{
			Color = color;
			return this;
		}

		public SpriteObject SetOrigin(Vector2 origin)
		{
			Origin = origin;
			return this;
		}

		public SpriteObject SetDepth(float depth)
		{
			Depth = depth;
			return this;
		}

		public override void Draw2D(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				Texture,
				new Vector2(Position.X, Position.Y),
				null,
				Color,
				Rotation.Z,
				Origin,
				new Vector2(Scale.X, Scale.Y),
				SpriteEffects.None,
				Depth
			);
		}
	}
}