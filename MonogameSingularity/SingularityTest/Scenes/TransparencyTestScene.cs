using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.GameObjects;

namespace SingularityTest.Scenes
{
	public class TransparencyTestScene : GameScene, ITransparent
	{
		public TransparencyTestScene(SingularityGame game) : base(game, "transparency", 2, 0, 0f)
		{
			SetAbsoluteCameraTarget(new Vector3());
			SetCameraPosition(new Vector3(2, 2, 2));
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			AddObject(new GameObject().AddScript((scene, o, arg3) =>
			{
				if (InputManager.IsKeyPressed(Keys.Escape)) SceneManager.CloseScene();
			}));

			AddObject(new ModelObject("cubes/cube1").AddScript((scene, o, arg3) =>
			{
				o.AddRotation(MathHelper.TwoPi, 0, 0, arg3);
			}));
		}

		//public override void AddLightningToEffect(Effect effect)
		//{
		//}
	}
}