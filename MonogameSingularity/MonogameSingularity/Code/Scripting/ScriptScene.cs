﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Singularity.GameObjects;

namespace Singularity.Scripting
{
    public class ScriptScene : GameScene
    {
        public ScriptingTemplate Script;
        private string _scriptKey;

        public ScriptScene(SingularityGame game, string path, ScriptingTemplate script) 
            : base(game, 
                   path, 
                   sceneSize: script.GetSettings().SceneSize == null ? 16 : script.GetSettings().SceneSize.Value,
                   minPartition: script.GetSettings().MinPartition == null ? 2 : script.GetSettings().MinPartition.Value,
                   precision: script.GetSettings().Precision == null ? 0f : script.GetSettings().Precision.Value)
        {
            _scriptKey = path;
            Script = script;
            Script.Init(game);
        }

        protected override void AddGameObjects(int entranceId)
        {
            var objs = Script.AddGameObjects(entranceId);
            foreach (var gameObject in objs)
            {
                AddObject(gameObject);
            }

#if DEBUG
            AddObject(new EmptyGameObject().AddScript((scene, o, time) =>
                                                      {
                                                          if (!KeyboardManager.IsKeyPressed(Keys.O)) return;

                                                          ScriptManager.ReloadScript(_scriptKey);
                                                          SceneManager.ChangeScene(_scriptKey, entranceId);
                                                      }));
#endif
		}

        public override void AddLightningToEffect(Effect effect)
        {
            Script.AddLightningToEffect(effect);
        }
    }
}
