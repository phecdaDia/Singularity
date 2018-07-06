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
		public MappingTestObject()
		{
			_position = new Vector2();
			AddChild(new ModelObject("cubes/cube1").SetScale(0.05f, 0.05f, 0.05f));
		}

		public Texture2D WhitePixel { get; private set; }

		private Vector2 _position { get; set; }

		public override void Update(GameScene scene, GameTime gameTime)
		{
			_position = GetHierarchyPosition().GetScreenPosition(
				scene.GetViewMatrix(),
				scene.GetProjectionMatrix(),
				scene.Game.RenderTarget.Width,
				scene.Game.RenderTarget.Height
			);
		}

		public override void Draw2D(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(WhitePixel,
				new Rectangle((int) (_position.X - 0.5f * WhitePixel.Width), (int) (_position.Y - 0.5f * WhitePixel.Height),
					WhitePixel.Width, WhitePixel.Height), Color.White);
		}

		public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			base.LoadContent(contentManager, graphicsDevice);

			//this.WhitePixel = new Texture2D(graphicsDevice, 1, 1);
			//this.WhitePixel.SetData(new [] {Color.White});

			WhitePixel = ImageManager.GetTexture2D("fallback");
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			//this.WhitePixel.Dispose();
		}
	}
}