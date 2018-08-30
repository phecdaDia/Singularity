using Microsoft.Xna.Framework.Input;
using Singularity.Core.GameObjects;

namespace Singularity.Core.Scripting
{
	public class ScriptScene : GameScene
	{
		private readonly string _scriptKey;
		public ScriptingTemplate Script;

		public ScriptScene(SingularityGame game, string path, ScriptingTemplate script)
			: base(game,
				path,
				script.GetSettings().SceneSize == null ? 16 : script.GetSettings().SceneSize.Value,
				script.GetSettings().MinPartition == null ? 2 : script.GetSettings().MinPartition.Value,
				script.GetSettings().Precision == null ? 0f : script.GetSettings().Precision.Value)
		{
			this._scriptKey = path;
			this.Script = script;
			this.Script.Init(game);
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			var objs = this.Script.AddGameObjects(entranceId);
			foreach (var gameObject in objs) this.AddObject(gameObject);

#if DEBUG
			this.AddObject(new EmptyGameObject().AddScript((scene, o, time) =>
			{
				if (!KeyboardManager.IsKeyPressed(Keys.O)) return;

				ScriptManager.ReloadScript(this._scriptKey);
				SceneManager.ChangeScene(this._scriptKey, entranceId);
			}));
#endif
		}

		//public override void AddLightningToEffect(Effect effect)
		//{
		//    Script.AddLightningToEffect(effect);
		//}
	}
}