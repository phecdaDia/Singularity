using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	public class SceneManager
	{
		private static SceneManager Instance;

		private readonly Dictionary<String, GameScene> GameScenes;

		private readonly Stack<GameScene> SceneStack;

		public SceneManager()
		{
			Instance = this;

			this.GameScenes = new Dictionary<string, GameScene>();
			this.SceneStack = new Stack<GameScene>();
		}

		public static GameScene GetCurrentScene() => Instance._GetCurrentScene();

		/// <summary>
		/// Returns the current <seealso cref="GameScene"/> from the <see cref="SceneStack"/>
		/// </summary>
		/// <returns></returns>
		public GameScene _GetCurrentScene()
		{
			return SceneStack.Peek();
		}

		public static void AddSceneToStack(GameScene scene) => Instance._AddSceneToStack(scene);

		/// <summary>
		/// Adds a <seealso cref="GameScene"/> to the <see cref="SceneStack"/>
		/// </summary>
		/// <param name="scene"></param>
		public void _AddSceneToStack(GameScene scene)
		{
			scene.SetupScene();
			scene.LoadContent();
			this.SceneStack.Push(scene);
		}

		public static void AddSceneToStack(String sceneKey) => Instance._AddSceneToStack(sceneKey);

		/// <summary>
		/// Adds a <seealso cref="GameScene"/> to the <see cref="SceneStack"/><para />
		/// Requires that the <seealso cref="GameScene"/> has been registered. <seealso cref="RegisterScene"/>
		/// </summary>
		/// <param name="sceneKey"></param>
		public void _AddSceneToStack(String sceneKey)
		{
			if (!GameScenes.ContainsKey(sceneKey))
			{
				// SceneKey is not registered.
				// Will be ignored for now.

				return;
			}

			AddSceneToStack(GameScenes[sceneKey]);
		}

		public static void CloseScene() => Instance._CloseScene();

		/// <summary>
		/// Removed a <seealso cref="GameScene"/> from the <see cref="SceneStack"/>
		/// </summary>
		public void _CloseScene()
		{
			GameScene scene = this.SceneStack.Pop();
			scene.UnloadContent();

			// unload content from the scene.
			
		}

		public static Boolean RegisterScene(GameScene scene) => Instance._RegisterScene(scene);

		/// <summary>
		/// Registers a <seealso cref="GameScene"/> so it can be added to the <see cref="SceneStack"/><para/>
		/// Required before using <seealso cref="AddSceneToStack(String)"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public Boolean _RegisterScene(GameScene scene)
		{
			if (GameScenes.ContainsKey(scene.SceneKey)) return false; // Scene is either already registered or the key is double. 
			GameScenes[scene.SceneKey] = scene;
			return true;
		}

		/// <summary>
		/// Updates the upper <seealso cref="GameScene"/>
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			this._GetCurrentScene().Update(gameTime);
		}

		/// <summary>
		/// Draws the upper <seealso cref="GameScene"/>
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{
			this._GetCurrentScene().Draw(spriteBatch);
		}
	}
}
