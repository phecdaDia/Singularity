using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Core;

namespace Singularity.Examples.GameObjects
{
	public abstract class SpriteObject : GameObject
	{
		public SpriteObject()
		{
		}

		public SpriteObject(Texture2D texture) : this()
		{
			this.Texture = texture;
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
			this.Color = color;
			return this;
		}

		public SpriteObject SetOrigin(Vector2 origin)
		{
			this.Origin = origin;
			return this;
		}

		public SpriteObject SetDepth(float depth)
		{
			this.Depth = depth;
			return this;
		}

		public override void Draw2D(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				this.Texture,
				new Vector2(this.Position.X, this.Position.Y),
				null,
				this.Color,
				this.Rotation.Z,
				this.Origin,
				new Vector2(this.Scale.X, this.Scale.Y),
				SpriteEffects.None,
				this.Depth
			);
		}
	}
}