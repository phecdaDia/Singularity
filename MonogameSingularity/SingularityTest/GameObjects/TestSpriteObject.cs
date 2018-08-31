using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity;
using Singularity.Core;
using Singularity.Core.GameObjects;
using Singularity.Examples.GameObjects;

namespace SingularityTest.GameObjects
{
	public class TestSpriteObject : SpriteObject
	{
		private SpriteFont TestFont;

		public override void Update(GameScene scene, GameTime gameTime)
		{
		}

		public override void Draw2D(SpriteBatch spriteBatch)
		{
			if (TestFont == null) return;

			var pos = GetHierarchyPosition();

			spriteBatch.DrawString(TestFont, $"Test String!", new Vector2(pos.X, pos.Y), Color.White);
		}

		public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			base.LoadContent(contentManager, graphicsDevice);
			TestFont = contentManager.Load<SpriteFont>("testfont");
		}
	}
}