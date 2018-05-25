using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Singularity.GameObjects;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Scripting
{
	/// <summary>
	/// Scene which loads Script
	///</summary>
	public class ScriptLoadingScene : GameScene
	{
		enum State
		{
			Loading,
			Done
		}

		private State _state;
		private string _scriptPath;
		private Assembly _curAssembly;
		private GameScene _newScene;
		private Type _loadingScreenType;
		private LoadingScreenTemplate _loadingScreen;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="game">reference to used SingularityGame</param>
		/// <param name="pathToLoadingScript">Path to script - can be relative or absolut</param>
		/// <param name="currentAssembly">Assembly from which this is called - Assembly.GetExecutingAssembly()</param>
		/// <param name="loadingScreen">Type of Loadingscreen to use</param>
		public ScriptLoadingScene(SingularityGame game, string pathToLoadingScript, Assembly currentAssembly,
			Type loadingScreen) : base(game, "ScriptLoading")
		{
			//CHeck loadingScreen Type
			if (loadingScreen == null || !loadingScreen.IsSubclassOf(typeof(LoadingScreenTemplate)))
				throw new Exception("LoadingScreen null or not subclass of LoadingScreenTemplate");

			//Store rest
			_loadingScreenType = loadingScreen;
			_state = State.Loading;
			_scriptPath = pathToLoadingScript;
			_curAssembly = currentAssembly;

			//Activate Loading screen
			_loadingScreen = (LoadingScreenTemplate) Activator.CreateInstance(loadingScreen);
			_loadingScreen.Init(Game);

			//Load script async
			Task.Run(new Action(RunScript));
		}

		public void RunScript()
		{
			/* Check if script exists.
			 * Read it
			 * Load it with Roslyn - injecting mscorlib, Monogame.Framework, Singularity & Current Assembly
			 * Compile it
			 * get the type of the ScriptingTemplate in script
			 * instanciate it
			 * Tell main thread that finished
			 */
			if (!File.Exists(_scriptPath))
				throw new Exception("Script does not exist");
			var scriptCode = File.ReadAllText(_scriptPath);

			var script = CSharpScript.Create(scriptCode, ScriptOptions.Default
				.WithReferences(Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(Game)),
					_curAssembly)
			);
			script.Compile();

			var scriptType = (Type) script.RunAsync().Result.ReturnValue;

			var scriptScene = (ScriptingTemplate) Activator.CreateInstance(scriptType);
			scriptScene.Init(Game);

			_newScene = new ScriptScene(Game, scriptScene, _scriptPath, _curAssembly, _loadingScreenType);
			_state = State.Done;
		}

		protected override void AddGameObjects(int entranceId)
		{
			AddObject(new EmptyGameObject().AddScript((scene, o, gameTime) =>
			{
				//CHeck if loading finished. if done -> close loading and open script-scene
				if (this._state == State.Done)
				{
					SceneManager.CloseScene();
					SceneManager.AddSceneToStack(_newScene, entranceId);
				}
			}));

			//Apply GameObjects from LoadingScreen
			var gameObjects = new List<GameObject>();
			_loadingScreen.AddGameObjects(gameObjects);
			foreach (var gameObject in gameObjects)
			{
				AddObject(gameObject);
			}
		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			_loadingScreen.AddLightningToEffect(effect);
		}
	}
}