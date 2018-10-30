using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Collisions;
using Singularity.GameObjects;

namespace SingularityTest.Scenes
{
	public class RayTestScene : GameScene
	{
		private int LastMouseX, LastMouseY;

		public RayTestScene(SingularityGame game, string sceneKey) : base(game, sceneKey, 8, 2, 0.0f)
		{
			ScenePauseEvent += (sender, args) =>
			{
				LastMouseX = Mouse.GetState().X;
				LastMouseY = Mouse.GetState().Y;
			};
			SceneResumeEvent += (sender, args) => Mouse.SetPosition(LastMouseX, LastMouseY);
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			//AddObject(new CollidableModelObject("cubes/cube1")
			//	.SetPosition(-10.5f, 0, 0)
			//	.SetScale(1, 7.5f, 7.5f)
			//	.SetCollision(new BoxCollision(new Vector3(-0.5f), new Vector3(0.5f)))
			//	.AddScript((gs, go, time) => { go.AddRotation(0, (float) time.ElapsedGameTime.TotalSeconds, 0); })
			//);

			AddObject(new CollidableModelObject("planes/plane1")
				.SetPosition(-20, 0, 0)
				.SetCollision(new PlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8)))
				.SetRotation(0, 0, -MathHelper.PiOver2)
			);

			//AddObject(new BasicCamera());

			SetAbsoluteCamera(new Vector3(), new Vector3(-10, 0, 0));

			AddObject(new ModelObject("cubes/cube2").SetPosition(-1, 0, 0).SetScale(0.025f).AddScript((scene, obj, time) =>
			{
				if (InputManager.IsKeyDown(Keys.W))
					obj.AddPosition(0, 1, 0, time);

				if (InputManager.IsKeyDown(Keys.S))
					obj.AddPosition(0, -1, 0, time);

				if (InputManager.IsKeyDown(Keys.A))
					obj.AddPosition(0, 0, 1, time);

				if (InputManager.IsKeyDown(Keys.D))
					obj.AddPosition(0, 0, -1, time);

				if (InputManager.IsKeyDown(Keys.M))
				{
					// cast a ray

					var rcp = CollideRay(new Ray(obj.GetHierarchyPosition(), new Vector3(-0.5f, 0.5f, 0)));

					if (rcp.DidCollide)
					{
						Console.WriteLine($"We do collide!");
						SpawnObject(new ModelObject("cubes/cube3").SetScale(0.05f).SetPosition(rcp.Position));
						Console.WriteLine($"Final position: {rcp.Position} ({(rcp.Position - obj.Position).Length()})");
					}
				}
			}));
		}

		//public override void AddLightningToEffect(Effect effect)
		//{

		//}
	}
}