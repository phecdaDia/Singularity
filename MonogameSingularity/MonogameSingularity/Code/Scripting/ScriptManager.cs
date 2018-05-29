using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Scripting
{
    public static class ScriptManager
    {
        private static bool   _isSetUp = false;
        private static string _loadingKey;
        private static Dictionary<string, ScriptData> _scriptList;

        public static void SetUp<TLoadingScene>(params object[] argumentsForLoadingScene)
            where TLoadingScene : GameScene, ILoadingScreen
        {
            _loadingKey =
                SceneManager.RegisterScene((GameScene) Activator.CreateInstance(typeof(TLoadingScene),
                                                                                argumentsForLoadingScene));
            _scriptList = new Dictionary<string, ScriptData>();
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
                        _AddScript(path);
                        break;
					case AddBehavior.ThrowException:
                        throw new Exception("Script with path \"" + path + "\" already added");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _AddScript(path);
        }

        private static void _AddScript(string path)
        {
            _scriptList.Add(path, new ScriptData(){IsLoaded = false, IsRegistered = false, Template = null, Scene = null});
        }

        public static void LoadScript(AddBehavior existsAlready = AddBehavior.Ignore, LoadBehavior alreadyLoaded = LoadBehavior.Ignore, params string[] paths)
        {
            CheckSetUp();

            SceneManager.AddSceneToStack(_loadingKey);
            //TODO LOAD

        }

        public static void LoadAllScripts(LoadBehavior alreadyLoaded = LoadBehavior.Ignore)
        {
            CheckSetUp();
            SceneManager.AddSceneToStack(_loadingKey);
            Task.Run(() =>
                     {
                         foreach (var key in _scriptList.Keys)
                         {
                         }
                     });
        }

        public static void RegisterScript(params string[] paths)
        {

        }

        public static void RegisterAllScripts()
        {

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

    internal struct ScriptData
    {
        public ScriptingTemplate Template;
        public ScriptScene Scene;
        public bool IsLoaded;
        public bool IsRegistered;

        public static implicit operator ScriptingTemplate(ScriptData obj) => obj.Template;
    }
}