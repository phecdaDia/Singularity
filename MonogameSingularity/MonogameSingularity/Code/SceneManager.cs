using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	public class SceneManager
	{
		private Dictionary<String, GameScene> GameScenes;

		private String CurrentSceneKey;

		public SceneManager()
		{

		}

		public Boolean RegisterScene(GameScene scene)
		{
			if (GameScenes.ContainsKey(scene.SceneKey)) return false; // Scene is either already registered or the key is double. 
			GameScenes[scene.SceneKey] = scene;
			return true;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			this.GameScenes[this.CurrentSceneKey].Draw(spriteBatch);
		}
	}
}
