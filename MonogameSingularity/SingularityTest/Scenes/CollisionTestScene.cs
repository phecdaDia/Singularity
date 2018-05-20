using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Collisions;
using Singularity.Collisions.Multi;
using Singularity.GameObjects;
using SingularityTest.GameObjects;

namespace SingularityTest.Scenes
{
	public class CollisionTestScene : GameScene
	{
		public CollisionTestScene(SingularityGame game) : base(game, "collision", 10, -2, 0.1f)
		{

		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			effect.EnableDefaultLighting();
		}

		protected override void AddGameObjects()
		{
			AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.Escape)) scene.Game.Exit();
			}));

			AddObject(new BasicCamera().Set3DEnabled(true).SetCameraTarget(new Vector3(0, 0, 1)).SetPosition(0, 1, -50).SetEnableCollision(false));


			// build the cube

			var planeCollision = new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1);

			AddObject(
				new CollidableModelObject("planes/plane1")
					.SetPosition(-8, 0, 0)
					.SetCollision(new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1))
					.SetRotation(0, 0, -MathHelper.PiOver2)
			);

			AddObject(
				new CollidableModelObject("planes/plane1")
					.SetPosition(8, 0, 0)
					.SetCollision(new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1))
					.SetRotation(0, 0, MathHelper.PiOver2)
			);

			AddObject(
				new CollidableModelObject("planes/plane1")
					.SetPosition(0, 8, 0)
					.SetCollision(new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1))
					.SetRotation(0, 0, MathHelper.Pi)
			);

			AddObject(
				new CollidableModelObject("planes/plane1")
					.SetPosition(0, -8, 0)
					.SetCollision(new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1))
					.SetRotation(0, 0, 0)
			);

			AddObject(
				new CollidableModelObject("planes/plane1")
					.SetPosition(0, 0, 8)
					.SetCollision(new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1))
					.SetRotation(-MathHelper.PiOver2, 0, 0)
			);

			AddObject(
				new CollidableModelObject("planes/plane1")
					.SetPosition(0, 0, -8)
					.SetCollision(new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8), (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1))
					.SetRotation(MathHelper.PiOver2, 0, 0)
			);

			// add some objects to the cube



			// add balls

			Random random = new Random();

			for (int i = 0; i <= 20; i++)
			{
				AddObject(new TestBallObject().SetInertia((float) (5f * random.NextDouble()), 0, (float)(5f * random.NextDouble())).SetPosition((float)(5f * random.NextDouble()), (float)(5f * random.NextDouble()), (float)(5f * random.NextDouble())));
			}

			//for (float x = 15; x >= -15; x -= 2.5f)
			//	AddObject(new TestBallObject().SetPosition(x, 10 + Math.Abs(x) / 2.0f, 0));


		}
	}
}
