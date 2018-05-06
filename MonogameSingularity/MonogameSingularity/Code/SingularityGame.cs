using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Singularity.Code.Utilities;

namespace Singularity.Code
{
	using System.IO;
	using System.Threading.Tasks;

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

	    protected RenderTarget2D RenderTarget;
		private RenderTarget2D _tempRenderTarget;
		private Texture2D _lastFrame;
		public List<Func<GameTime, Texture2D, ScreenEffectData>> ScreenEffectList = new List<Func<GameTime, Texture2D, ScreenEffectData>>();

		public SingularityGame()
		{
			this.SceneManager = new SceneManager();
			this.ModelManager = new ModelManager(Content);
			ImageManager.SetContentManager(Content);

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

		/// <summary>
		/// Initialize Basic Structures
		/// </summary>
	    protected override void Initialize()
	    {
            RenderTarget = new RenderTarget2D(GraphicsDevice, 1920,1080, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 8, RenderTargetUsage.DiscardContents);
			
	        this.SpriteBatch = new SpriteBatch(GraphicsDevice);
			base.Initialize();
	    }

		
		/// <summary>
		/// Basic Draw loop
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Draw(GameTime gameTime)
		{
            //Draw everything to RenderTarget instead of the Screen
            GraphicsDevice.SetRenderTarget(RenderTarget);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			ResetGraphic();
			BeginRender3D();

			// Add Drawing stuff here!
			SpriteBatch.Begin(SpriteSortMode.FrontToBack);

			SceneManager.Draw(this.SpriteBatch);

			SpriteBatch.End();
      
			//Apply each function for 2D Screenwide effects
			foreach (var func in ScreenEffectList)
			{
				var data = func.Invoke(gameTime, RenderTarget);

				_tempRenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 8, RenderTargetUsage.DiscardContents);
				GraphicsDevice.SetRenderTarget(_tempRenderTarget);
				GraphicsDevice.Clear(Color.CornflowerBlue);

				SpriteBatch.Begin();
				SpriteBatch.Draw(texture: RenderTarget, 
					destinationRectangle: new Rectangle(), 
					sourceRectangle: new Rectangle(), 
					color: data.Color, 
					rotation: data.Rotation, 
					origin:data.Origin, 
					effects: data.Effect, 
					layerDepth: 0);
				SpriteBatch.End();

				RenderTarget.Dispose();
				RenderTarget = _tempRenderTarget;
			}

			_lastFrame = RenderTarget;

            //Draw RenderTarget to Screen
            GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();

		    SpriteBatch.Draw(texture: RenderTarget,
		                     destinationRectangle: new Rectangle(new Point(0, 0),
		                                                         new Point(GraphicsDeviceManager.PreferredBackBufferWidth,
		                                                                   GraphicsDeviceManager.PreferredBackBufferHeight)),
		                     color: Color.White);

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

		public Texture2D GetScreenShot() => _lastFrame;

		public void SaveScreenshot(string location) =>
			SaveScreenshot(location, DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss.png"));

		public void SaveScreenshot(string location, string name)
		{
			Task.Run(() =>
			{
				Stream stream = File.Create(location + name);
				_lastFrame.SaveAsPng(stream, _lastFrame.Width, _lastFrame.Height);
				stream.Dispose();
			});
		}
	}
}
