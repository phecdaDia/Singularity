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

namespace SingularityTest.GameObjects
{
	public class TestSpriteObject : SpriteObject
	{
		private SpriteFont TestFont;

		public TestSpriteObject() : base()
		{
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{

		}

		public override void Draw2D(SpriteBatch spriteBatch)
		{
			if (this.TestFont == null) return;

			var pos = this.GetHierarchyPosition();

			spriteBatch.DrawString(this.TestFont, $"Test String!", new Vector2(pos.X, pos.Y), Color.White);
		}

		public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			base.LoadContent(contentManager, graphicsDevice);
			this.TestFont = contentManager.Load<SpriteFont>("testfont");
		}
	}
}
