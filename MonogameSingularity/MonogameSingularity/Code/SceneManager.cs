using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	public class SceneManager
	{
		private SingularityGame Game;

		private static SceneManager Instance;

		private readonly Dictionary<String, GameScene> GameScenes;

		private readonly Stack<GameScene> SceneStack;

		private Boolean IsSceneClosing = false;

		private Queue<GameScene> SceneQueue;

		public SceneManager(SingularityGame game)
		{
			this.Game = game;
			Instance = this;

			this.GameScenes = new Dictionary<string, GameScene>();
			this.SceneStack = new Stack<GameScene>();
			this.SceneQueue = new Queue<GameScene>();
		}

		public static GameScene GetCurrentScene() => Instance._GetCurrentScene();

		/// <summary>
		/// Returns the current <seealso cref="GameScene"/> from the <see cref="SceneStack"/>
		/// </summary>
		/// <returns></returns>
		public GameScene _GetCurrentScene()
		{
			if (this.SceneStack.Count == 0) return null;

			return SceneStack.Peek();
		}

		public static void AddSceneToStack(GameScene scene, int entranceId = 0) => Instance._AddSceneToStack(scene, entranceId);

		/// <summary>
		/// Adds a <seealso cref="GameScene"/> to the <see cref="SceneStack"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="entranceId"></param>
		public void _AddSceneToStack(GameScene scene, int entranceId)
		{
			// add scene to queue first..
			// pause the current scene
			this._GetCurrentScene()?.OnScenePause();

			scene.SetupScene(entranceId);
			scene.LoadContent();
			this.SceneQueue.Enqueue(scene);
		}

		public static void AddSceneToStack(String sceneKey, int entranceId = 0) => Instance._AddSceneToStack(sceneKey, entranceId);

		/// <summary>
		/// Adds a <seealso cref="GameScene"/> to the <see cref="SceneStack"/><para />
		/// Requires that the <seealso cref="GameScene"/> has been registered. <seealso cref="RegisterScene"/>
		/// </summary>
		/// <param name="sceneKey"></param>
		/// <param name="entranceId"></param>
		public void _AddSceneToStack(String sceneKey, int entranceId)
		{
			if (!GameScenes.ContainsKey(sceneKey))
			{
				// SceneKey is not registered.
				// Will be ignored for now.

				return;
			}

			AddSceneToStack(GameScenes[sceneKey], entranceId);
		}

		public static void CloseScene() => Instance._CloseScene();

		/// <summary>
		/// Removed a <seealso cref="GameScene"/> from the <see cref="SceneStack"/>
		/// </summary>
		public void _CloseScene()
		{
			this.IsSceneClosing = true;
		}

		public static string RegisterScene(GameScene scene) => Instance._RegisterScene(scene);

		/// <summary>
		/// Registers a <seealso cref="GameScene"/> so it can be added to the <see cref="SceneStack"/><para/>
		/// Required before using <seealso cref="AddSceneToStack(String)"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public string _RegisterScene(GameScene scene)
		{
			if (GameScenes.ContainsKey(scene.SceneKey)) return null; // Scene is either already registered or the key is double. 
			GameScenes[scene.SceneKey] = scene;
			return scene.SceneKey;
		}

		/// <summary>
		/// Updates the upper <seealso cref="GameScene"/>
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			// add scenes to the stack
			while (this.SceneQueue.Count > 0)
			{
				this.SceneStack.Push(this.SceneQueue.Dequeue());
			}

			var scene = this._GetCurrentScene();

			if (scene == null)
			{
				// close game, as there are no scenes left
				Game.Exit();

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

			while (this.SceneQueue.Count > 0)
			{
				this.SceneStack.Push(this.SceneQueue.Dequeue());
			}
		}

		/// <summary>
		/// Draws the upper <seealso cref="GameScene"/>
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{
			var scene = this._GetCurrentScene();

			if (scene == null)
			{
				// close game, as there are no scenes left
				Game.Exit();

				return;
			}

			scene.Draw(spriteBatch);
		}
	}
}
