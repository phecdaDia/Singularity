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

		public ScriptLoadingScene(SingularityGame game, string sceneKey, string pathToLoadingScript, Assembly currentAssembly) : base(game, sceneKey)
		{
			_state = State.Loading;
			_scriptPath = pathToLoadingScript;
			_curAssembly = currentAssembly;
			Task.Run(new Action(RunScript));
		}

		public void RunScript()
		{
			var scriptText = File.ReadAllText(_scriptPath);
			var script = CSharpScript.Create(scriptText, ScriptOptions.Default
				.WithReferences(Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(Game)), _curAssembly)
				);
			script.Compile();
			var testScene = (Type) script.RunAsync().Result.ReturnValue;

			var scene = (GameScene) Activator.CreateInstance(testScene, args: Game);
			_newScene = scene;
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
		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
		}
	}
}
