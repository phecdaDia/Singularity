using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Scripting
{
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;

	using GameObjects;

	using Microsoft.CodeAnalysis.CSharp.Scripting;
	using Microsoft.CodeAnalysis.Scripting;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	using Utilities;

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

		public ScriptLoadingScene(SingularityGame game, string pathToLoadingScript, Assembly currentAssembly, Type loadingScreen) : base(game, "ScriptLoading")
		{
			if(loadingScreen == null || !loadingScreen.IsSubclassOf(typeof(LoadingScreenTemplate)))
				throw new Exception("LoadingScreen null or not subclass of LoadingScreenTemplate");

			_loadingScreenType = loadingScreen;
			_state = State.Loading;
			_scriptPath = pathToLoadingScript;
			_curAssembly = currentAssembly;

			_loadingScreen = (LoadingScreenTemplate) Activator.CreateInstance(loadingScreen);
			_loadingScreen.Init(Game);

			//Load script async
			Task.Run(new Action(RunScript));
		}

		public void RunScript()
		{
			if(!File.Exists(_scriptPath))
				throw new Exception("Script does not exist");
			var scriptCode = File.ReadAllText(_scriptPath);

			var script = CSharpScript.Create(scriptCode, ScriptOptions.Default
				.WithReferences(Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(Game)), _curAssembly)
				);
			script.Compile();

			var scriptType = (Type) script.RunAsync().Result.ReturnValue;

			var scriptScene = (ScriptingTemplate) Activator.CreateInstance(scriptType);

			_newScene = new ScriptScene(Game, scriptScene, _scriptPath, _curAssembly, _loadingScreenType);
			_state = State.Done;
		}

		protected override void AddGameObjects()
		{
			AddObject(new EmptyGameObject().AddScript((scene, o, gameTime) =>
			{
				if (this._state == State.Done)
				{
					Game.SceneManager.CloseScene();
					Game.SceneManager.AddSceneToStack(_newScene);
				}
			}));


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
