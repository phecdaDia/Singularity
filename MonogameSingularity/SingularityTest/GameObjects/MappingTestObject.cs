using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity;
using Singularity.GameObjects;
using Singularity.Helpers;

namespace SingularityTest.GameObjects
{
	public class MappingTestObject : GameObject
	{
		public Texture2D WhitePixel { get; private set; }

		private Vector2 _position { get; set; }

		public MappingTestObject()
		{
			this._position = new Vector2();
			this.AddChild(new ModelObject("cubes/cube1").SetScale(0.05f, 0.05f, 0.05f));
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{

			_position = this.GetHierarchyPosition().GetScreenPosition(
				scene.GetViewMatrix(), 
				scene.GetProjectionMatrix(),
				scene.Game.RenderTarget.Width,
				scene.Game.RenderTarget.Height
			);
		}

		protected override void Draw2D(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(this.WhitePixel,
				new Rectangle((int) (this._position.X - 0.5f * this.WhitePixel.Width), (int) (this._position.Y - 0.5f * this.WhitePixel.Height),
					this.WhitePixel.Width, this.WhitePixel.Height), Color.White);

		}

		public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			base.LoadContent(contentManager, graphicsDevice);

			//this.WhitePixel = new Texture2D(graphicsDevice, 1, 1);
			//this.WhitePixel.SetData(new [] {Color.White});

			this.WhitePixel = ImageManager.GetTexture2D("fallback");

		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			//this.WhitePixel.Dispose();
		}
	}
}
