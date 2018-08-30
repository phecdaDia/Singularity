using Singularity.Core.GameObjects;

namespace Singularity.Core.Scripting
{
	internal class StartingScene : GameScene
	{
		private readonly int _entranceId;

		private readonly string _start;

		public StartingScene(SingularityGame game, string start, int entranceID) : base(game,
			"startingScene|" + start + "|" + entranceID)
		{
			this._start = start;
			this._entranceId = entranceID;
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			this.AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
			{
				if (LoadingScreenTemplate.DoneLoading)
				{
					SceneManager.ChangeScene(this._start, this._entranceId);
					LoadingScreenTemplate.DoneLoading = false;
				}
			}));
		}

		//public override void AddLightningToEffect(Effect effect)
		//{
		//}
	}
}