using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Core;
using Singularity.Core.GameObjects;

namespace SingularityTest.Scenes
{
	public class LightTestScene : LightGameScene
	{
		public LightTestScene(SingularityGame game) : base(game, "light", 4096)
		{
			SetLightDirection(new Vector3(-7, -4, -7));

			SetProjectionMatrix(Matrix.CreateOrthographic(120, 120, 0.01f, 256.0f));
			SetAbsoluteCamera(new Vector3(-20, 20, 20), new Vector3(0, 0, 0));
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			var width = 40.0f;

			var i = 0;

			while (width >= 1.0f)
			{
				AddObject(new ModelObject("cubes/cube1")
					.SetPosition(20 - width / 2, -0.5f + i, 20 - width / 2)
					.SetScale(width, 1, width)
				);

				i++;
				width /= 1.1f;
			}

			AddObject(new EmptyGameObject()
				.SetPosition(0, 10, 0)
				.AddChild(
					new ModelObject("sphere")
						.SetScale(0.5f)
						.SetPosition(5.0f, 0.0f)
				).AddScript((scene, o, arg3) => o.AddRotation(0.0f, 1.0f, 0.0f, arg3))
			);

			//AddObject(new ModelObject("sphere").SetPosition(0, 10, 0));


			AddObject(new BasicCamera().Set3DEnabled(true).SetPosition(0, 10, 10)
				.AddScript((scene, obj, time) =>
				{
					// enable or disable 3d.
					if (KeyboardManager.IsKeyPressed(Keys.F1)) ((BasicCamera) obj).Set3DEnabled(!((BasicCamera) obj).Is3DEnabled);

					// some more movement options
					if (KeyboardManager.IsKeyDown(Keys.Q))
						obj.AddPosition(0, 1, 0, time);
					if (KeyboardManager.IsKeyDown(Keys.E))
						obj.AddPosition(0, -1, 0, time);

					((LightGameScene) scene).SetLightPosition(obj.Position + new Vector3(70, 40, 70));
				}));
		}
	}
}