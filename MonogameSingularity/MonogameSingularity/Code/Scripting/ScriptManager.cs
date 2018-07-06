using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singularity.Scripting
{
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.Xna.Framework;

    public static class ScriptManager
    {
        private static bool   _isSetUp = false;
        private static string _loadingKey;
        private static Dictionary<string, ScriptData> _scriptList;
        private static Assembly _runningAssembly;
        private static SingularityGame _game;

        public static void SetUp<TLoadingScene>(Assembly currentAssembly, SingularityGame game)
            where TLoadingScene : LoadingScreenTemplate
        {
            _loadingKey =
                SceneManager.RegisterScene((GameScene) Activator.CreateInstance(typeof(TLoadingScene), game));
            _scriptList = new Dictionary<string, ScriptData>();
            _runningAssembly = currentAssembly;
            _game = game;
            _isSetUp = true;
        }

        public static bool IsSetUp() => _isSetUp;

        private static void CheckSetUp()
        {
            if(!_isSetUp)
                throw new Exception("ScriptManager not set up!");
        }

        public static void AddScript(AddBehavior behavior = AddBehavior.Ignore, params string[] paths)
        {
            CheckSetUp();

            foreach (var path in paths)
            {
                AddScript(path, behavior);
            }

        }

        private static void AddScript(string path, AddBehavior behavior)
        {
            if (_scriptList.ContainsKey(path))
            {
                switch (behavior)
                {
					case AddBehavior.Ignore:
                        return;
					case AddBehavior.Overwrite:
					    _scriptList.Remove(path);
                        _AddScriptToDict(path);
                        break;
					case AddBehavior.ThrowException:
                        throw new Exception("Script with path \"" + path + "\" already added");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _AddScriptToDict(path);
        }

        private static void _AddScriptToDict(string path)
        {
            if(!File.Exists(path))
                throw new Exception("Script does not exist \"" + path + "\"");

			_scriptList.Add(path, new ScriptData(){IsLoaded = false, IsRegistered = false, Template = null, Scene = null});
        }

        public static void LoadAllScripts(AddBehavior existsAlready = AddBehavior.Ignore, 
                                          LoadBehavior alreadyLoaded = LoadBehavior.Reload)
        {
            CheckSetUp();
            _LoadScripts(_scriptList.Keys, alreadyLoaded);
        }

        private static void _LoadScripts(IEnumerable<string> scriptPaths, LoadBehavior alredyLoaded = LoadBehavior.Reload)
        {
            SceneManager.AddSceneToStack(_loadingKey);
            Task.Run(() =>
                     {
                         var loadingScreen = (LoadingScreenTemplate) SceneManager.GetScene(_loadingKey);
                         var tempDic = new Dictionary<string, ScriptingTemplate>();

                         foreach (var scriptPath in scriptPaths)
                         {
                             var scriptData = _scriptList[scriptPath];
                             if(scriptData.IsLoaded)
                                 switch (alredyLoaded)
                                 {
									 case LoadBehavior.Ignore:
                                         continue;
									 case LoadBehavior.Reload:
									     break;
									 case LoadBehavior.ThrowException:
                                         throw new Exception("Script already loaded \"" + scriptPath + "\"");
                                     default:
                                         throw new ArgumentOutOfRangeException(nameof(alredyLoaded), alredyLoaded, null);
                                 }

                             loadingScreen.CurrentlyLoading(scriptPath);

                             var scriptCode = File.ReadAllText(scriptPath);
                             var script = CSharpScript.Create(scriptCode,
                                                              ScriptOptions
                                                                  .Default.WithReferences(Assembly.GetCallingAssembly(),
                                                                                          Assembly
                                                                                              .GetExecutingAssembly(),
                                                                                          Assembly
                                                                                              .GetAssembly(typeof(Game
                                                                                                           )),
                                                                                          _runningAssembly));
                             script.Compile();
                             var tempScriptType = script.RunAsync().Result.ReturnValue;
                             if (!(tempScriptType is Type))
                             {
                                 throw new Exception("Return type of Script \"" + scriptPath + "\n is not correct");
                             }

                             var scriptType = (Type)tempScriptType;

                             tempDic[scriptPath] = (ScriptingTemplate) Activator.CreateInstance(scriptType);
                         }


                         foreach (var tempDicKey in tempDic.Keys)
                         {
                             var data = _scriptList[tempDicKey];
                             data.Template = tempDic[tempDicKey];
                             data.IsLoaded = true;
                             _scriptList[tempDicKey] = data;
                         }
                         loadingScreen.LoadingDone();
                     });
        }

        public static void RegisterAllScripts()
        {
            var paths = _scriptList.Keys;
            var tempDict = new Dictionary<string, ScriptScene>();

            foreach (var path in paths)
            {
                var scriptData = _scriptList[path];
                var scriptScene = new ScriptScene(_game, path, scriptData.Template);
                SceneManager.RegisterScene(scriptScene, SceneManager.RegisterBehavior.Overwrite);
                tempDict[path] = scriptScene;
            }

            foreach (var key in tempDict.Keys)
            {
                var scriptData = _scriptList[key];
                scriptData.Scene = tempDict[key];
                scriptData.IsRegistered = true;
                _scriptList[key] = scriptData;
            }
        }

        public static void ReloadScript(string path)
        {
            CheckSetUp();
            var scriptData = _scriptList[path];
            var scriptCode = File.ReadAllText(path);
            var script =
                CSharpScript.Create(scriptCode, ScriptOptions.Default.WithReferences(Assembly.GetCallingAssembly(),
                                                                                         Assembly
                                                                                             .GetExecutingAssembly(),
                                                                                         Assembly
                                                                                             .GetAssembly(typeof(Game
                                                                                                          )),
                                                                                         _runningAssembly));
            script.Compile();
            var scriptType = (Type) script.RunAsync().Result.ReturnValue;
            scriptData.Template = (ScriptingTemplate) Activator.CreateInstance(scriptType);

            scriptData.Scene = new ScriptScene(_game, path, scriptData.Template);
            SceneManager.RegisterScene(scriptData.Scene, SceneManager.RegisterBehavior.Overwrite);
            _scriptList[path] = scriptData;
        }

        public static void LoadAllAndStart(string start, int entranceId = 0)
        {
            CheckSetUp();
            var LoadingScene = (LoadingScreenTemplate)SceneManager.GetScene(_loadingKey);
            LoadingScreenTemplate.DoneLoading = false;

            SceneManager.AddSceneToStack(new StartingScene(_game, start, entranceId));
			SceneManager.AddSceneToStack(_loadingKey);

            Task.Run(() =>
                     {
                         Thread.Sleep(100);

                         var paths = new string[_scriptList.Keys.Count];
                         _scriptList.Keys.CopyTo(paths, 0);

                         foreach (var path in paths)
                         {
                             LoadingScene.CurrentlyLoading(path);

                             var scriptData = _scriptList[path];
                             var scriptCode = File.ReadAllText(path);
                             var script =
                                 CSharpScript.Create(scriptCode, ScriptOptions.Default.WithReferences(Assembly.GetCallingAssembly(),
                                                                                                      Assembly
                                                                                                          .GetExecutingAssembly(),
                                                                                                      Assembly
                                                                                                          .GetAssembly(typeof(Game
                                                                                                                       )),
                                                                                                      _runningAssembly));
                             script.Compile();
                             var scriptType = (Type)script.RunAsync().Result.ReturnValue;
                             scriptData.Template = (ScriptingTemplate)Activator.CreateInstance(scriptType);
                             scriptData.IsLoaded = true;

                             _scriptList[path] = scriptData;
                         }

                         LoadingScene.LoadingDone();
					 });
        }

        /// <summary>
		/// Behavior when adding scripts with key already added
		/// </summary>
        public enum AddBehavior
        {
            /// <summary>
			/// Throw Exception
			/// </summary>
            ThrowException,
            /// <summary>
			/// Do nothing - continue
			/// </summary>
            Ignore,
            /// <summary>
			/// Overwrite existing
			/// </summary>
            Overwrite
        }

        public enum LoadBehavior
        {
            ThrowException,
            Reload,
            Ignore
        }
    }
}