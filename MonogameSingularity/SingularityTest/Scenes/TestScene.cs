using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Code;
using Singularity.Code.GameObjects;

namespace SingularityTest.Scenes
{
	public class TestScene : GameScene
	{
		public TestScene(SingularityGame game) : base(game, "test")
		{
			this.SetCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

		}

		protected override void AddGameObjects()
		{
			AddObject(new BasicCamera().SetPosition(0, 0, 10).AddScript((scene, obj, time) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.F1)) ((BasicCamera)obj).Set3DEnabled(!((BasicCamera)obj).Is3DEnabled);


				if (KeyboardManager.IsKeyDown(Keys.Q)) obj.AddPosition(new Vector3(0, 1, 0) * (float)time.ElapsedGameTime.TotalSeconds);
				if (KeyboardManager.IsKeyDown(Keys.E)) obj.AddPosition(new Vector3(0, -1, 0) * (float)time.ElapsedGameTime.TotalSeconds);

				if (KeyboardManager.IsKeyDown(Keys.F2)) scene.SpawnObject(new CollidableModelObject("unit-cube-small")
					.SetPosition(obj.Position + new Vector3(0, 0, -5)));
			}));

			AddObject(new CollidableModelObject("sphere"));

			//AddObject(new CollidableModelObject("longplayer").SetPosition(5, 0, 10));

			//for (int i = -10; i <= 10; i++)
			//{
			//	AddObject(new ModelObject("cube-025").SetPosition(30, 0, 5 * i));
			//	AddObject(new CollidableModelObject("cube-100").SetPosition(30, 0, 5 * i));
			//}

			//AddObject(new CollidableModelObject("player").SetPosition(5, 0, 10).SetScale(0.5f, 0.5f, 0.5f));

			//AddObject(new CollidableModelObject("sphere").SetCollisionMode(CollisionMode.BoundingBox).SetPosition(new Vector3(5, 20, 50)).AddScript(
			//	(scene, obj, time) =>
			//	{
			//		obj.AddPosition((float) -time.ElapsedGameTime.TotalSeconds / 2.0f, 0, 0);
			//	}));

			//AddObject(new CollidableModelObject("unit-cube-small").SetCollisionMode(CollisionMode.BoundingBox).SetPosition(new Vector3(5, 3, 50)));


			//AddObject(new ModelObject("wood_table").SetPosition(new Vector3(0, 0, -2f)));

		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f); // some diffuse light
			effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // 
			effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
			//effect.AmbientLightColor = new Vector3(1f, 1f, 1f); // Add some overall ambient light.
			effect.AmbientLightColor = new Vector3(0.15f, 0.15f, 0.215f); // Add some overall ambient light.
		}
	}
}
