using System;
using System.CodeDom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	public class SingularityGame : Game
	{
		public static readonly String SINGULARITY_VERSION = "v0.06";

		protected readonly SceneManager SceneManager;
		protected readonly ModelManager ModelManager;
		//protected GameWindow GameWindow;

		protected readonly GraphicsDeviceManager GraphicsDeviceManager;
		protected SpriteBatch SpriteBatch;

	    protected RenderTarget2D RenderTarget;

		public SingularityGame()
		{
			this.SceneManager = new SceneManager();
			this.ModelManager = new ModelManager(Content);

			this.GraphicsDeviceManager = new GraphicsDeviceManager(this)
			{
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
			};
			Content.RootDirectory = "Content";
		}

	    public SingularityGame(int width, int height) : base()
	    {
	        GraphicsDeviceManager.PreferredBackBufferHeight = height;
	        GraphicsDeviceManager.PreferredBackBufferWidth = width;
            GraphicsDeviceManager.ApplyChanges();
	    }

	    protected override void Initialize()
	    {
            RenderTarget = new RenderTarget2D(GraphicsDevice, 1920,1080, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 8, RenderTargetUsage.DiscardContents);
	        this.SpriteBatch = new SpriteBatch(GraphicsDevice);
			base.Initialize();
	    }

	    protected override void Draw(GameTime gameTime)
		{
            //Draw everything to RenderTarget instead of the Screen
            GraphicsDevice.SetRenderTarget(RenderTarget);

			GraphicsDevice.Clear(Color.CornflowerBlue);
			ResetGraphic();
			BeginRender3D();

			// Add Drawing stuff here!
			SceneManager.Draw(this.SpriteBatch);
			
            //Draw RenderTarget to Screen to add effects later
            GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin();

			//TODO: Add Class/Method/Posibility to easily change how this is drawn

		    SpriteBatch.Draw(texture: RenderTarget,
		                     destinationRectangle: new Rectangle(new Point(0, 0),
		                                                         new Point(GraphicsDeviceManager.PreferredBackBufferWidth,
		                                                                   GraphicsDeviceManager.PreferredBackBufferHeight)),
		                     color: Color.White);

            SpriteBatch.End();
		}

		protected void ResetGraphic()
		{
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.None;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

		}

		public void BeginRender3D()
		{
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			this.SceneManager.Update(gameTime);
			KeyboardManager.Update();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			// Create a new SpriteBatch, which can be used to draw textures.
			this.SpriteBatch = new SpriteBatch(GraphicsDevice);
		}
	}
}
