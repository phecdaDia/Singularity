using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Singularity.GameObjects;

namespace Singularity.Scripting
{
	/// <summary>
	/// Scene in which Script will be embedded
	/// </summary>
	public class ScriptScene : GameScene
	{
		private static int _instanceCounter = 0;

		private ScriptingTemplate _script;
		private string _pathToScript;
		private Assembly _currentAssembly;
		private Type _loadingScreen;

		/* Takes Game, the compliled & instanciated script, the path to it, the running Assembly ([Name].exe) and typereference to Loadingscreen
		 * Uses Settings in script to define the Scene
		 * everything else is stored & the script initialted
		 */
		public ScriptScene(SingularityGame game, ScriptingTemplate script, string pathToScript, Assembly currentAssembly, Type loadingScreen) 
			: base(game, 
				sceneKey: string.IsNullOrEmpty(script.GetSettings().SceneKey) ? "ScriptedScene" + (++_instanceCounter) : script.GetSettings().SceneKey, 
				sceneSize: script.GetSettings().SceneSize?? 16, 
				minPartition: script.GetSettings().MinPartition ?? 2,
				precision: script.GetSettings().Precision ?? 0f)
		{
			_script = script;
			_pathToScript = pathToScript;
			_currentAssembly = currentAssembly;
			_loadingScreen = loadingScreen;
		}

		protected override void AddGameObjects()
		{
			//Reload script in Debug mode with O
#if DEBUG
			AddObject(new EmptyGameObject().AddScript((scene, obj, time) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.O))
				{
					SceneManager.CloseScene();
					SceneManager.AddSceneToStack(new ScriptLoadingScene(Game, _pathToScript, _currentAssembly, _loadingScreen));
				}
			}));
#endif
			//Get all objects from script and use them
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
