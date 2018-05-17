using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Singularity.GameObjects;
using Singularity.Utilities;

namespace Singularity.Scripting
{
	public class ScriptScene : GameScene
	{
		private ScriptingTemplate _script;
		private string _pathToScript;
		private Assembly _currentAssembly;
		private Type _loadingScreen;

		public ScriptScene(SingularityGame game, ScriptingTemplate script, string pathToScript, Assembly currentAssembly, Type loadingScreen) 
			: base(game, 
				sceneKey: string.IsNullOrEmpty(script.GetSettings().SceneKey) ? "ScriptedScene" : script.GetSettings().SceneKey, 
				sceneSize: script.GetSettings().SceneSize?? 16, 
				minPartition: script.GetSettings().MinPartition ?? 2,
				precision: script.GetSettings().Precision ?? 0f)
		{
			_script = script;
			_pathToScript = pathToScript;
			_currentAssembly = currentAssembly;
			_loadingScreen = loadingScreen;
			script.Init(game);
		}

		protected override void AddGameObjects()
		{
#if DEBUG
			AddObject(new EmptyGameObject().AddScript((scene, obj, time) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.O))
				{
					Game.SceneManager.CloseScene();
					Game.SceneManager.AddSceneToStack(new ScriptLoadingScene(Game, _pathToScript, _currentAssembly, _loadingScreen));
				}
			}));
#endif
			var gameObjects = new List<GameObject>();
			_script.AddGameObjects(gameObjects);

			foreach (var gameObject in gameObjects)
			{
				AddObject(gameObject);
			}

		}

		/// <summary>
		/// Adds lightning to the <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="effect"></param>
		public override void AddLightningToEffect(BasicEffect effect)
		{
			_script.AddLightningToEffect(effect);
		}
	}
}
