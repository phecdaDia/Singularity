using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Utilities;

namespace Singularity
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

		

		public SingularityGame()
		{
			Instance = this;

			SceneManager = new SceneManager(this);
			ImageManager.SetContentManager(Content);

			GraphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1280,
				PreferredBackBufferHeight = 720,
				PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
			};
			Content.RootDirectory = "Content";
			
		}

		public static ContentManager GetContentManager()
		{
			return GetGameInstance().Content;
		}

		private static SingularityGame GetGameInstance()
		{
			return Instance;
		}

		public SingularityGame(int width, int height) : this()
		{
			GraphicsDeviceManager.PreferredBackBufferHeight = height;
			GraphicsDeviceManager.PreferredBackBufferWidth = width;
			GraphicsDeviceManager.ApplyChanges();
		}

		/// <summary>
		///     Initialize Basic Structures
		/// </summary>
		protected override void Initialize()
		{
			SceneManager.SetSceneRender(new RenderTarget2D(GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color,
				DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents));

			RenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color,
				DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
			_tempRenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color,
				DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
			SpriteBatch = new SpriteBatch(GraphicsDevice);

			base.Initialize();
		}


		/// <summary>
		///     Basic Draw loop
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Draw(GameTime gameTime)
		{
			OnDrawEvent(gameTime);

			//Draw everything to RenderTarget instead of the Screen
			GraphicsDevice.SetRenderTarget(RenderTarget);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			ResetGraphic();
			BeginRender3D();

			// Add Drawing stuff here!

			SceneManager.Draw(SpriteBatch);

			//Apply each function for 2D Screenwide effects
			foreach (var func in ScreenEffectList)
			{
				var data = func.Invoke(gameTime, RenderTarget);

				GraphicsDevice.SetRenderTarget(_tempRenderTarget);
				GraphicsDevice.Clear(Color.Black);

				SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
					DepthStencilState.Default, RasterizerState.CullNone);
				SpriteBatch.Draw(RenderTarget,
					data.Destination,
					data.Source,
					data.Color ?? Color.White,
					data.Rotation ?? 0,
					data.Origin,
					data.Effect ?? SpriteEffects.None,
					0);
				SpriteBatch.End();

				var tempRenderSwitchHelper = RenderTarget;
				RenderTarget = _tempRenderTarget;
				_tempRenderTarget = tempRenderSwitchHelper;

				if (data.IsDone)
					_removalList.Add(func);
			}

			_lastFrame = RenderTarget;


			//Draw RenderTarget to Screen
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);

			SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
				DepthStencilState.Default, RasterizerState.CullNone);

			SpriteBatch.Draw(RenderTarget,
				new Rectangle(new Point(0, 0),
					new Point(GraphicsDeviceManager.PreferredBackBufferWidth,
						GraphicsDeviceManager.PreferredBackBufferHeight)),
				new Rectangle(new Point(0, 0),
					new Point(RenderTarget.Width, RenderTarget.Height)),
				Color.White);

			SpriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		///     Basic Update loop.
		/// </summary>
		/// <param name="gameTime">DeltaTime</param>
		protected sealed override void Update(GameTime gameTime)
		{
			OnPreUpdateEvent(gameTime);

			//Remove finished ScreenEffects
			ScreenEffectList.RemoveAll(f => _removalList.Contains(f));
			_removalList.Clear();

			base.Update(gameTime);
			SceneManager.Update(gameTime);
			KeyboardManager.Update();

			OnPostUpdateEvent(gameTime);
		}

		/// <summary>
		///     Resets graphics
		/// </summary>
		protected void ResetGraphic()
		{
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.None;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
		}

		/// <summary>
		///     Sets up BlendState and DepthStencilState for 3d rendering
		/// </summary>
		public void BeginRender3D()
		{
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		/// <summary>
		///     Loads content
		///     Automatically initialized the SpriteBatch
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();

			// Create a new SpriteBatch, which can be used to draw textures.
			SpriteBatch = new SpriteBatch(GraphicsDevice);
		}

		public Texture2D GetScreenShot()
		{
			return _lastFrame;
		}

		public void SaveScreenshot(string location)
		{
			SaveScreenshot(location, DateTime.Now.ToString("yyy_MM_dd_hh_mm_ss"));
		}

		public void SaveScreenshot(string location, string name)
		{
			Stream stream = File.Create(location + "\\" + name + ".png");
			_lastFrame.SaveAsPng(stream, _lastFrame.Width, _lastFrame.Height);
			stream.Dispose();
		}

		#region Events

		protected event EventHandler<GameTime> PreUpdateEvent;

		private void OnPreUpdateEvent(GameTime e)
		{
			PreUpdateEvent?.Invoke(this, e);
		}

		protected event EventHandler<GameTime> PostUpdateEvent;

		private void OnPostUpdateEvent(GameTime e)
		{
			PostUpdateEvent?.Invoke(this, e);
		}

		protected event EventHandler<GameTime> DrawEvent;

		private void OnDrawEvent(GameTime e)
		{
			DrawEvent?.Invoke(this, e);
		}

		#endregion
	}
}