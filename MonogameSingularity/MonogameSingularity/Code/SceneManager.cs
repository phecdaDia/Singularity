using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Core
{
	public class SceneManager
	{
		public enum RegisterBehavior
		{
			Ignore,
			Overwrite,
			ThrowException
		}

		private static SceneManager Instance;

		private readonly Dictionary<string, GameScene> GameScenes;

		private readonly Stack<GameScene> SceneStack;
		private readonly SingularityGame Game;

		private bool IsSceneClosing;

		private bool IsStackClearing;

		private readonly Queue<GameScene> SceneQueue;

		private RenderTarget2D SceneRender;

		public SceneManager(SingularityGame game)
		{
			this.Game = game;
			Instance = this;

			this.GameScenes = new Dictionary<string, GameScene>();
			this.SceneStack = new Stack<GameScene>();
			this.SceneQueue = new Queue<GameScene>();
		}

		/// <summary>
		///     Returns the current <seealso cref="GameScene" /> from the <see cref="SceneStack" />
		/// </summary>
		/// <returns></returns>
		public static GameScene GetCurrentScene()
		{
			return Instance._GetCurrentScene();
		}

		public GameScene _GetCurrentScene()
		{
			if (this.SceneStack.Count == 0) return null;

			return this.SceneStack.Peek();
		}

		/// <summary>
		///     Adds a <seealso cref="GameScene" /> to the <see cref="SceneStack" />
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="entranceId"></param>
		public static void AddSceneToStack(GameScene scene, int entranceId = 0)
		{
			Instance._AddSceneToStack(scene, entranceId);
		}

		public void _AddSceneToStack(GameScene scene, int entranceId)
		{
			// add scene to queue first..
			// pause the current scene
			this._GetCurrentScene()?.OnScenePause();

			scene.LoadContent();
			scene.SetupScene(this.SceneStack.Count > 0 ? this.SceneStack.Peek() : null, entranceId);
			this.SceneQueue.Enqueue(scene);
		}

		/// <summary>
		///     Adds a <seealso cref="GameScene" /> to the <see cref="SceneStack" />
		///     <para />
		///     Requires that the <seealso cref="GameScene" /> has been registered. <seealso cref="RegisterScene" />
		/// </summary>
		/// <param name="sceneKey"></param>
		/// <param name="entranceId"></param>
		public static void AddSceneToStack(string sceneKey, int entranceId = 0)
		{
			Instance._AddSceneToStack(sceneKey, entranceId);
		}

		public void _AddSceneToStack(string sceneKey, int entranceId)
		{
			if (!this.GameScenes.ContainsKey(sceneKey)) throw new Exception("SceneKey \"" + sceneKey + "\" was not registered");

			AddSceneToStack(this.GameScenes[sceneKey], entranceId);
		}

		/// <summary>
		///     Removed a <seealso cref="GameScene" /> from the <see cref="SceneStack" />
		/// </summary>
		public static void CloseScene()
		{
			Instance._CloseScene();
		}

		public void _CloseScene()
		{
			this.IsSceneClosing = true;
		}

		/// <summary>
		///     Registers a <seealso cref="GameScene" /> so it can be added to the <see cref="SceneStack" />
		///     <para />
		///     Required before using <seealso cref="AddSceneToStack(string)" />
		///     Returns Key of GameScene
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public static string RegisterScene(GameScene scene, RegisterBehavior behavior = RegisterBehavior.Ignore)
		{
			return Instance._RegisterScene(scene, behavior);
		}

		public string _RegisterScene(GameScene scene, RegisterBehavior behavior = RegisterBehavior.Ignore)
		{
			if (this.GameScenes.ContainsKey(scene.SceneKey))
				switch (behavior)
				{
					case RegisterBehavior.Ignore:
						return null;
					case RegisterBehavior.Overwrite:
						break;
					case RegisterBehavior.ThrowException:
						throw new Exception($"Scene \"{scene.SceneKey}\" already registered");
					default:
						throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
				}
			this.GameScenes[scene.SceneKey] = scene;
			return scene.SceneKey;
		}

		/// <summary>
		///     Updates the upper <seealso cref="GameScene" />
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			// add scenes to the stack
			while (this.SceneQueue.Count > 0) this.SceneStack.Push(this.SceneQueue.Dequeue());

			var scene = this._GetCurrentScene();

			if (scene == null)
			{
				// close game, as there are no scenes left
				this.Game.Exit();

				return;
			}

			scene.Update(gameTime);

			// close scene is it's schedules to close

			if (this.IsSceneClosing)
			{
				this.IsSceneClosing = false;
				this.SceneStack.Pop();
				scene.UnloadContent();
				// unload content from the scene.

				// get current scene and call OnResume if possible
				// check if we don't add any new scenes
				if (this.SceneQueue.Count == 0)
					this._GetCurrentScene()?.OnSceneResume();
			}

			if (this.IsStackClearing)
			{
				this.IsStackClearing = false;
				foreach (var gameScene in this.SceneStack) gameScene.UnloadContent();
				this.SceneStack.Clear();
			}

			while (this.SceneQueue.Count > 0) this.SceneStack.Push(this.SceneQueue.Dequeue());
		}

		/// <summary>
		///     Draws the upper <seealso cref="GameScene" />
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (this.SceneStack.Count == 0)
			{
				this.Game.Exit();
				return;
			}

			var drawScenes = new Stack<GameScene>();
			while (this.SceneStack.Count > 0 && this.SceneStack.Peek() is ITransparent)
				// add scenes we want to draw.
				drawScenes.Push(this.SceneStack.Pop());

			// add one more scene, if there is any
			if (this.SceneStack.Count > 0)
				drawScenes.Push(this.SceneStack.Pop()); // this is the lowest scene

			// now work these scenes from down to top
			while (drawScenes.Count > 0)
			{
				this.SceneStack.Push(drawScenes.Pop()); // puts scene back on the sceneStack so it doesn't go missing
				this.SceneStack.Peek().Draw(spriteBatch, this.SceneRender); // draws the entire scene to our RenderTarget

				this.Game.GraphicsDevice.SetRenderTarget(this.Game.RenderTarget);

				spriteBatch.Begin(); // draws the scene on top of everything else that was already drawn
				spriteBatch.Draw( // causes a layered effect. We can see the scenes below
					this.SceneRender,
					new Rectangle(0, 0, this.Game.RenderTarget.Width, this.Game.RenderTarget.Height),
					new Rectangle(0, 0, this.SceneRender.Width, this.SceneRender.Height),
					Color.White
				);
				spriteBatch.End();
			}
		}

		/// <summary>
		///     Returns the <seealso cref="GameScene" /> for known key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static GameScene GetScene(string key)
		{
			return Instance._GetScene(key);
		}

		private GameScene _GetScene(string key)
		{
			return this.GameScenes.ContainsKey(key) ? this.GameScenes[key] : null;
		}

		/// <summary>
		///     Clear entire Stack
		/// </summary>
		public static void ClearStack()
		{
			Instance._ClearStack();
		}

		private void _ClearStack()
		{
			this.IsStackClearing = true;
		}

		/// <summary>
		///     Remove CurrentScene and add new Scene to Stack
		/// </summary>
		/// <param name="newScene"></param>
		/// <param name="entranceId"></param>
		/// <param name="clearStack"></param>
		public static void ChangeScene(GameScene newScene, int entranceId = 0, bool clearStack = false)
		{
			Instance._ChangeScene(newScene, entranceId, clearStack);
		}

		private void _ChangeScene(GameScene newScene, int entranceId = 0, bool clearStack = false)
		{
			if (clearStack)
				this._ClearStack();
			this._CloseScene();
			this._AddSceneToStack(newScene, entranceId);
		}

		/// <summary>
		///     Remove CurrentScene and add new Scene to Stack
		///     Scene should already be registred
		/// </summary>
		/// <param name="newSceneKey"></param>
		/// <param name="entranceId"></param>
		/// <param name="clearStack"></param>
		public static void ChangeScene(string newSceneKey, int entranceId = 0, bool clearStack = false)
		{
			Instance._ChangeScene(newSceneKey, entranceId, clearStack);
		}

		private void _ChangeScene(string newSceneKey, int entranceId = 0, bool clearStack = false)
		{
			if (clearStack)
				this._ClearStack();
			this._CloseScene();
			this._AddSceneToStack(newSceneKey, entranceId);
		}

		public static void SetSceneRender(RenderTarget2D render)
		{
			Instance.SceneRender = render;
		}
	}
}