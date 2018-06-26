using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.GameObjects;

namespace SingularityTest.Scenes
{
	public class TransparencyTestScene : GameScene, ITransparent
	{
		public TransparencyTestScene(SingularityGame game) : base(game, "transparency", 2, 0, 0f)
		{

			this.SetAbsoluteCameraTarget(new Vector3());
			this.SetCameraPosition(new Vector3(2, 2, 2));
		}

		protected override void AddGameObjects(int entranceId)
		{
			AddObject(new EmptyGameObject().AddScript(((scene, o, arg3) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.Escape)) SceneManager.CloseScene();
			})));

			AddObject(new ModelObject("cubes/cube1").AddScript(((scene, o, arg3) =>
				{
					o.AddRotation(MathHelper.TwoPi * (float) arg3.ElapsedGameTime.TotalSeconds, 0, 0);
				})));
		}

		//public override void AddLightningToEffect(Effect effect)
		//{
		//}
	}
}
