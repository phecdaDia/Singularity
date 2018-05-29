using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
    using System.Runtime.CompilerServices;

    public class SceneManager
	{
		private SingularityGame Game;

		private static SceneManager Instance;

		private readonly Dictionary<String, GameScene> GameScenes;

		private readonly Stack<GameScene> SceneStack;

		private bool IsSceneClosing = false;

	    private bool IsStackClearing = false;

		private Queue<GameScene> SceneQueue;

		public SceneManager(SingularityGame game)
		{
			this.Game = game;
			Instance = this;

			this.GameScenes = new Dictionary<string, GameScene>();
			this.SceneStack = new Stack<GameScene>();
			this.SceneQueue = new Queue<GameScene>();
		}

	    /// <summary>
	    /// Returns the current <seealso cref="GameScene"/> from the <see cref="SceneStack"/>
	    /// </summary>
	    /// <returns></returns>
		public static GameScene GetCurrentScene() => Instance._GetCurrentScene();

		public GameScene _GetCurrentScene()
		{
			if (this.SceneStack.Count == 0) return null;

			return SceneStack.Peek();
		}

	    /// <summary>
	    /// Adds a <seealso cref="GameScene"/> to the <see cref="SceneStack"/>
	    /// </summary>
	    /// <param name="scene"></param>
	    /// <param name="entranceId"></param>
		public static void AddSceneToStack(GameScene scene, int entranceId = 0) => Instance._AddSceneToStack(scene, entranceId);

		public void _AddSceneToStack(GameScene scene, int entranceId)
		{
			// add scene to queue first..
			// pause the current scene
			this._GetCurrentScene()?.OnScenePause();

			scene.SetupScene(entranceId);
			scene.LoadContent();
			this.SceneQueue.Enqueue(scene);
		}

	    /// <summary>
	    /// Adds a <seealso cref="GameScene"/> to the <see cref="SceneStack"/><para />
	    /// Requires that the <seealso cref="GameScene"/> has been registered. <seealso cref="RegisterScene"/>
	    /// </summary>
	    /// <param name="sceneKey"></param>
	    /// <param name="entranceId"></param>
		public static void AddSceneToStack(string sceneKey, int entranceId = 0) => Instance._AddSceneToStack(sceneKey, entranceId);

		public void _AddSceneToStack(string sceneKey, int entranceId)
		{
			if (!GameScenes.ContainsKey(sceneKey))
			{
				// SceneKey is not registered.
				// Will be ignored for now.
                throw new Exception("SceneKey \"" + sceneKey + "\" was not registered");
				//return;
			}

			AddSceneToStack(GameScenes[sceneKey], entranceId);
		}

	    /// <summary>
	    /// Removed a <seealso cref="GameScene"/> from the <see cref="SceneStack"/>
	    /// </summary>
		public static void CloseScene() => Instance._CloseScene();

		public void _CloseScene()
		{
			this.IsSceneClosing = true;
		}

	    /// <summary>
	    /// Registers a <seealso cref="GameScene"/> so it can be added to the <see cref="SceneStack"/><para/>
	    /// Required before using <seealso cref="AddSceneToStack(String)"/>
	    /// Returns Key of GameScene
	    /// </summary>
	    /// <param name="scene"></param>
	    /// <returns></returns>
		public static string RegisterScene(GameScene scene) => Instance._RegisterScene(scene);

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

		    if (this.IsStackClearing)
		    {
		        this.IsStackClearing = false;
		        foreach (var gameScene in SceneStack)
		        {
		            gameScene.UnloadContent();
		        }
                SceneStack.Clear();
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

		/// <summary>
		/// Returns the <seealso cref="GameScene"/> for known key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static GameScene GetScene(string key) => Instance._GetScene(key);

	    private GameScene _GetScene(string key) => GameScenes.ContainsKey(key) ? GameScenes[key] : null;

        /// <summary>
		/// Clear entire Stack
		/// </summary>
	    public static void ClearStack() => Instance._ClearStack();

	    private void _ClearStack()
	    {
	        this.IsStackClearing = true;
	    }

        /// <summary>
		/// Remove CurrentScene and add new Scene to Stack
		/// </summary>
		/// <param name="newScene"></param>
		/// <param name="entranceId"></param>
		/// <param name="clearStack"></param>
	    public static void ChangeScene(GameScene newScene, int entranceId = 0, bool clearStack = false) =>
	        Instance._ChangeScene(newScene, entranceId, clearStack);

	    private void _ChangeScene(GameScene newScene, int entranceId = 0, bool clearStack = false)
	    {
            if(clearStack)
                _ClearStack();
            _CloseScene();
            _AddSceneToStack(newScene, entranceId);
	    }

		/// <summary>
		/// Remove CurrentScene and add new Scene to Stack
		/// Scene should already be registred
		/// </summary>
		/// <param name="newSceneKey"></param>
		/// <param name="entranceId"></param>
		/// <param name="clearStack"></param>
		public static void ChangeScene(string newSceneKey, int entranceId = 0, bool clearStack = false) =>
	        Instance._ChangeScene(newSceneKey, entranceId, clearStack);

	    private void _ChangeScene(string newSceneKey, int entranceId = 0, bool clearStack = false)
	    {
            if(clearStack)
                _ClearStack();
            _CloseScene();
            _AddSceneToStack(newSceneKey, entranceId);
	    }
	}
}
