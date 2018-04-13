using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	public class SceneManager
	{
		private readonly Dictionary<String, GameScene> GameScenes;

		private readonly Stack<GameScene> SceneStack;

		public SceneManager()
		{
			this.GameScenes = new Dictionary<string, GameScene>();
			this.SceneStack = new Stack<GameScene>();
		}

		public GameScene GetCurrentScene()
		{
			return SceneStack.Peek();
		}

		public void AddSceneToStack(GameScene scene)
		{
			scene.SetupScene();
			this.SceneStack.Push(scene);
		}

		public void AddSceneToStack(String sceneKey)
		{
			if (!GameScenes.ContainsKey(sceneKey))
			{
				// SceneKey is not registered.
				// Will be ignored for now.

				return;
			}

			AddSceneToStack(GameScenes[sceneKey]);
		}

		public void CloseScene()
		{
			GameScene scene = this.SceneStack.Pop();
			
		}

		public Boolean RegisterScene(GameScene scene)
		{
			if (GameScenes.ContainsKey(scene.SceneKey)) return false; // Scene is either already registered or the key is double. 
			GameScenes[scene.SceneKey] = scene;
			return true;
		}

		public void Update(GameTime gameTime)
		{
			this.GetCurrentScene().Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			this.GetCurrentScene().Draw(spriteBatch);
		}
	}
}
