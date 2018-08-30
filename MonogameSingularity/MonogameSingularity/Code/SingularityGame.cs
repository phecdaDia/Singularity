using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Core.Utilities;

namespace Singularity.Core
{
	public class SingularityGame : Game
	{
		/// <summary>
		///     Current Version of Singularity
		/// </summary>
		public static readonly string SINGULARITY_VERSION = "v0.08";

		private static SingularityGame Instance;

		private readonly List<Func<GameTime, Texture2D, ScreenEffectData>> _removalList =
			new List<Func<GameTime, Texture2D, ScreenEffectData>>();

		protected readonly GraphicsDeviceManager GraphicsDeviceManager;

		private readonly SceneManager SceneManager;

		public readonly List<Func<GameTime, Texture2D, ScreenEffectData>> ScreenEffectList =
			new List<Func<GameTime, Texture2D, ScreenEffectData>>();

		private Texture2D _lastFrame;
		private RenderTarget2D _tempRenderTarget;

		public RenderTarget2D RenderTarget;
		protected SpriteBatch SpriteBatch;

		public static double MinimumFramerate = 0.04d;

		private bool inSizeChange = false;
		private int ratioWidth = 16;
		private int ratioHeight = 9;

		public SingularityGame()
		{
			Instance = this;

			this.SceneManager = new SceneManager(this);

			this.GraphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1280,
				PreferredBackBufferHeight = 720,
				PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
			};
			this.Content.RootDirectory = "Content";
		}

		public static ContentManager GetContentManager()
		{
			return GetGameInstance().Content;
		}

		private static SingularityGame GetGameInstance()
		{
			return Instance;
		}

		public SingularityGame(int width, int height, int ratioWidth = 16, int ratioHeight = 9) : this()
		{
			this.GraphicsDeviceManager.PreferredBackBufferHeight = height;
			this.GraphicsDeviceManager.PreferredBackBufferWidth = width;
			this.GraphicsDeviceManager.ApplyChanges();
			this.ratioWidth = ratioWidth;
			this.ratioHeight = ratioHeight;
		}

		/// <summary>
		///     Initialize Basic Structures
		/// </summary>
		protected override void Initialize()
		{
			SceneManager.SetSceneRender(new RenderTarget2D(this.GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color,
				DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents));

			this.RenderTarget = new RenderTarget2D(this.GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color,
				DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
			this._tempRenderTarget = new RenderTarget2D(this.GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color,
				DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
			this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);

			base.Initialize();
			this.Window.ClientSizeChanged += (sender, args) =>
			                            {
				                            if (this.inSizeChange) return;

				                            this.inSizeChange = true;
				                            this.GraphicsDeviceManager.PreferredBackBufferHeight =
					                            this.Window.ClientBounds.Height;
				                            this.GraphicsDeviceManager.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
				                            this.GraphicsDeviceManager.ApplyChanges();

				                            this.inSizeChange = false;
			                            };
		}


		/// <summary>
		///     Basic Draw loop
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Draw(GameTime gameTime)
		{
			this.OnDrawEvent(gameTime);

			//Draw everything to RenderTarget instead of the Screen
			this.GraphicsDevice.SetRenderTarget(this.RenderTarget);
			this.GraphicsDevice.Clear(Color.CornflowerBlue);

			this.ResetGraphic();
			this.BeginRender3D();

			// Add Drawing stuff here!

			this.SceneManager.Draw(this.SpriteBatch);

			//Apply each function for 2D Screenwide effects
			foreach (var func in this.ScreenEffectList)
			{
				var data = func.Invoke(gameTime, this.RenderTarget);

				this.GraphicsDevice.SetRenderTarget(this._tempRenderTarget);
				this.GraphicsDevice.Clear(Color.Black);

				this.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
					DepthStencilState.Default, RasterizerState.CullNone);
				this.SpriteBatch.Draw(this.RenderTarget,
					data.Destination,
					data.Source,
					data.Color ?? Color.White,
					data.Rotation ?? 0,
					data.Origin,
					data.Effect ?? SpriteEffects.None,
					0);
				this.SpriteBatch.End();

				var tempRenderSwitchHelper = this.RenderTarget;
				this.RenderTarget = this._tempRenderTarget;
				this._tempRenderTarget = tempRenderSwitchHelper;

				if (data.IsDone)
					this._removalList.Add(func);
			}

			this._lastFrame = this.RenderTarget;


			//Draw RenderTarget to Screen
			this.GraphicsDevice.SetRenderTarget(null);
			this.GraphicsDevice.Clear(Color.Black);

			this.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
				DepthStencilState.Default, RasterizerState.CullNone);

			var width = this.ratioWidth * this.GraphicsDeviceManager.PreferredBackBufferHeight / this.ratioHeight;

			this.SpriteBatch.Draw(this.RenderTarget,
				new Rectangle(new Point((this.GraphicsDeviceManager.PreferredBackBufferWidth - width)/2, 0),
					new Point(width,
						this.GraphicsDeviceManager.PreferredBackBufferHeight)),
				new Rectangle(new Point(0, 0),
					new Point(this.RenderTarget.Width, this.RenderTarget.Height)),
				Color.White);

			this.SpriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		///     Basic Update loop.
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Update(GameTime gameTime)
		{
			this.OnPreUpdateEvent(gameTime);

			InputManager.Update();

			//Remove finished ScreenEffects
			this.ScreenEffectList.RemoveAll(f => this._removalList.Contains(f));
			this._removalList.Clear();

			base.Update(gameTime);
			this.SceneManager.Update(gameTime);
			KeyboardManager.Update();

			this.OnPostUpdateEvent(gameTime);
		}

		/// <summary>
		///     Resets graphics
		/// </summary>
		protected void ResetGraphic()
		{
			this.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			this.GraphicsDevice.DepthStencilState = DepthStencilState.None;
			this.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			this.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
		}

		/// <summary>
		///     Sets up BlendState and DepthStencilState for 3d rendering
		/// </summary>
		public void BeginRender3D()
		{
			this.GraphicsDevice.BlendState = BlendState.Opaque;
			this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		/// <summary>
		///     Loads content
		///     Automatically initialized the SpriteBatch
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();

			// Create a new SpriteBatch, which can be used to draw textures.
			this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
		}

		public Texture2D GetScreenShot()
		{
			return this._lastFrame;
		}

		public void SaveScreenshot(string location)
		{
			this.SaveScreenshot(location, DateTime.Now.ToString("yyy_MM_dd_hh_mm_ss"));
		}

		public void SaveScreenshot(string location, string name)
		{
			Stream stream = File.Create(location + "\\" + name + ".png");
			this._lastFrame.SaveAsPng(stream, this._lastFrame.Width, this._lastFrame.Height);
			stream.Dispose();
		}

		public void SetRatio(int _ratioWidth, int _ratioHeight)
		{
			this.ratioWidth = _ratioWidth;
			this.ratioHeight = _ratioHeight;
		}

		#region Events

		protected event EventHandler<GameTime> PreUpdateEvent;

		private void OnPreUpdateEvent(GameTime e)
		{
			this.PreUpdateEvent?.Invoke(this, e);
		}

		protected event EventHandler<GameTime> PostUpdateEvent;

		private void OnPostUpdateEvent(GameTime e)
		{
			this.PostUpdateEvent?.Invoke(this, e);
		}

		protected event EventHandler<GameTime> DrawEvent;

		private void OnDrawEvent(GameTime e)
		{
			this.DrawEvent?.Invoke(this, e);
		}

		#endregion
	}
}