using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code.GameObjects
{
	public abstract class SpriteObject : GameObject
	{
		public Texture2D Texture { get; private set; }
		public Color Color { get; private set; }
		public Vector2 Origin { get; private set; }
		public float Depth { get; private set; }

		public SpriteObject(Texture2D texture) : base()
		{
			this.Texture = texture;
		}

		public SpriteObject(String textureKey) : this(ImageManager.GetTexture2D(textureKey))
		{}

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

		protected override void Draw2D(SpriteBatch spriteBatch)
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
