using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Code;
using Singularity.Code.Enum;
using Singularity.Code.GameObjects;

namespace SingularityTest
{
	public class TestScene : GameScene
	{
		public TestScene() : base("test")
		{
			this.SetCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

		}

		protected override void AddGameObjects()
		{
			AddObject(new BasicCamera().SetPosition(0, 0, 50).AddScript((scene, obj, time) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.F1)) ((BasicCamera)obj).Set3DEnabled(!((BasicCamera)obj).Is3DEnabled);


				if (KeyboardManager.IsKeyDown(Keys.Q)) obj.AddPosition(new Vector3(0, 0, 1) * (float)time.ElapsedGameTime.TotalSeconds);
				if (KeyboardManager.IsKeyDown(Keys.E)) obj.AddPosition(new Vector3(0, 0, -1) * (float)time.ElapsedGameTime.TotalSeconds);

				if (KeyboardManager.IsKeyDown(Keys.F2)) scene.SpawnObject(new CollidableModelObject("unit-cube-small")
					.SetCollisionMode(CollisionMode.BoundingBox)
					.SetPosition(obj.Position + new Vector3(0, 0, -5)));
			}));

			AddObject(new CollidableModelObject("sphere").SetCollisionMode(CollisionMode.BoundingBox).SetPosition(new Vector3(5, 20, 50)).AddScript(
				(scene, obj, time) =>
				{
					obj.AddPosition((float) -time.ElapsedGameTime.TotalSeconds / 2.0f, 0, 0);
				}));

			AddObject(new CollidableModelObject("unit-cube-small").SetCollisionMode(CollisionMode.BoundingBox).SetPosition(new Vector3(5, 3, 50)));


			//AddObject(new ModelObject("wood_table").SetPosition(new Vector3(0, 0, -2f)));

		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f); // some diffuse light
			effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // 
			effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
			effect.AmbientLightColor = new Vector3(0.15f, 0.15f, 0.215f); // Add some overall ambient light.
		}
	}
}
