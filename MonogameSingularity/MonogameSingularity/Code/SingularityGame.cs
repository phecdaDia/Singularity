using System;
using System.CodeDom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	public class SingularityGame : Game
	{
		/// <summary>
		/// Current Version of Singularity
		/// </summary>
		public static readonly String SINGULARITY_VERSION = "v0.06";
		
		protected readonly SceneManager SceneManager;
		protected readonly ModelManager ModelManager;

		protected readonly GraphicsDeviceManager GraphicsDeviceManager;
		protected SpriteBatch SpriteBatch;

		public SingularityGame()
		{
			this.SceneManager = new SceneManager();
			this.ModelManager = new ModelManager(Content);
			ImageManager.SetContentManager(Content);

			this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}
		
		/// <summary>
		/// Basic Draw loop
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Draw(GameTime gameTime)
		{

			GraphicsDevice.Clear(Color.CornflowerBlue);
			ResetGraphic();
			BeginRender3D();

			if (this.SpriteBatch == null) return;

			// Add Drawing stuff here!
			SpriteBatch.Begin(SpriteSortMode.FrontToBack);

			SceneManager.Draw(this.SpriteBatch);

			SpriteBatch.End();
		}

		/// <summary>
		/// Basic Update loop.
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			this.SceneManager.Update(gameTime);
			KeyboardManager.Update();
		}

		/// <summary>
		/// Resets graphics
		/// </summary>
		protected void ResetGraphic()
		{
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.None;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

		}

		/// <summary>
		/// Sets up BlendState and DepthStencilState for 3d rendering
		/// </summary>
		public void BeginRender3D()
		{
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		/// <summary>
		/// Loads content
		/// Automatically initialized the SpriteBatch
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();

			// Create a new SpriteBatch, which can be used to draw textures.
			this.SpriteBatch = new SpriteBatch(GraphicsDevice);
		}
	}
}
