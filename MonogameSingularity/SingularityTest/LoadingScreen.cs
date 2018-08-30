using Singularity;
using Singularity.Core;
using Singularity.Core.Scripting;
using SingularityTest.GameObjects;

namespace SingularityTest
{
	public class LoadingScreen : LoadingScreenTemplate
	{
		public LoadingScreen(SingularityGame game) : base(game)
		{
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			AddObject(new TestSpriteObject());
		}

		//public override void AddLightningToEffect(Effect effect)
		//{

		//}
	}
}