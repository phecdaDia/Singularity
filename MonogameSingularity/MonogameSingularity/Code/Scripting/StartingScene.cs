using Singularity.GameObjects;

namespace Singularity.Scripting
{
	internal class StartingScene : GameScene
	{
		private readonly int _entranceId;

		private readonly string _start;

		public StartingScene(SingularityGame game, string start, int entranceID) : base(game,
			"startingScene|" + start + "|" + entranceID)
		{
			_start = start;
			_entranceId = entranceID;
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			AddObject(new GameObject().AddScript((scene, o, arg3) =>
			{
				if (LoadingScreenTemplate.DoneLoading)
				{
					SceneManager.ChangeScene(_start, _entranceId);
					LoadingScreenTemplate.DoneLoading = false;
				}
			}));
		}

		//public override void AddLightningToEffect(Effect effect)
		//{
		//}
	}
}