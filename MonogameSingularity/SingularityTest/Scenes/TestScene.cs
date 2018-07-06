using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Collisions;
using Singularity.Collisions.Multi;
using Singularity.GameObjects;
using Singularity.Utilities;
using SingularityTest.GameObjects;
using SingularityTest.GameObjects.ChildTest;
using SingularityTest.ScreenEffect;

namespace SingularityTest.Scenes
{
	public class TestScene : LightGameScene
	{
		public TestScene(SingularityGame game) : base(game, "test", 4096)
		{
			//this.SetCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
			this.SceneResumeEvent += (s, e) => { Mouse.SetPosition(200, 200); };


			this.SetLightPosition(new Vector3(-50, 50, 0));
			this.SetLightDirection(new Vector3(1, -1, 0));

			this.SetProjectionMatrix(Matrix.CreateOrthographic(100, 100, 0.01f, 200f));
		}

		protected override void AddGameObjects(int entranceId)
		{
			// load the savegame
			Savegame sg = Savegame.GetSavegame();

			Mouse.SetPosition(200, 200);

			Vector3 camTarget = sg.IsValidSavegame ? sg.CameraTarget : new Vector3(-1, 0, 0);
			Vector3 camPosition = new Vector3();
			if (entranceId == 0)
			{
				camPosition = sg.IsValidSavegame ? sg.Position : new Vector3(0, 0, 10);
			}
			else if (entranceId == 1)
			{
				camPosition = new Vector3(10);
			}

			AddObject(new ModelObject("cubes/cube1").SetPosition(-51, 51, 0).SetScale(0.5f, 0.5f, 0.5f));

			AddObject(new BasicCamera()
				.Set3DEnabled(true)
				.SetCameraTarget(camTarget)
				.SetPosition(camPosition)
				.AddScript((scene, obj, time) =>
				{
					// enable or disable 3d.
					if (KeyboardManager.IsKeyPressed(Keys.F1)) ((BasicCamera)obj).Set3DEnabled(!((BasicCamera)obj).Is3DEnabled);

					// some more movement options
					if (KeyboardManager.IsKeyDown(Keys.Q)) obj.AddPosition(new Vector3(0, 5, 0), time);
					if (KeyboardManager.IsKeyDown(Keys.E)) obj.AddPosition(new Vector3(0, -5, 0), time);

					// screen effects

					if (KeyboardManager.IsKeyPressed(Keys.Y))
						Game.ScreenEffectList.Add(ShakeScreenEffect.GetNewShakeScreenEffect(0.5f, 4).GetEffectData);
					if (KeyboardManager.IsKeyPressed(Keys.X))
						Game.ScreenEffectList.Add(ColorScreenEffect.GetNewColorScreenEffect(1, Color.Red).GetEffectData);
					if (KeyboardManager.IsKeyPressed(Keys.C))
						Game.SaveScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));


					if (KeyboardManager.IsKeyPressed(Keys.O))
						SceneManager.AddSceneToStack("transparency");

					if (KeyboardManager.IsKeyPressed(Keys.R))
					{
						obj.SetEnableCollision(!obj.EnablePushCollision);
					}

					// savegame stuff
					sg.Position = obj.Position;
					sg.CameraTarget = ((BasicCamera)obj).GetCameraTarget();


					if (KeyboardManager.IsKeyDown(Keys.M))
					{
						// run a ray and get the closest collision
						Ray r = new Ray(obj.Position, ((BasicCamera)obj).GetCameraTarget());
						var rcp = scene.CollideRay(r);
						if (rcp.DidCollide)
						{
							//Console.WriteLine($"Did collide! @{rcp.Position}");
							SpawnObject(new ModelObject("cubes/cube1").SetPosition(rcp.Position).SetScale(0.1f).AddScript(
								(GameScene s, GameObject o, GameTime t) =>
								{
									if (KeyboardManager.IsKeyPressed(Keys.OemSemicolon)) s.RemoveObject(o);
								}));
						}
					}

				})
			);

			AddObject(new ModelObject("slopes/slope1").SetPosition(-1.5f, -0.85f, -4));
			AddObject(new ModelObject("slopes/slope2").SetPosition(-0.5f, -0.85f, -2));
			AddObject(new ModelObject("slopes/slope3").SetPosition(0, -0.85f, 0));
			AddObject(new ModelObject("slopes/slope4").SetPosition(0, -0.35f, 2));
			AddObject(new ModelObject("slopes/slope5").SetPosition(0, 0.65f, 4));

			//AddObject(new TestBallObject().SetPosition(2, 3, 0));

			AddObject(new CollidableModelObject("cubes/cube5").SetPosition(0, -9, 0)
				//.SetRotation(0, 0, 0.4f)
				.SetCollision(new BoxCollision(new Vector3(-8), new Vector3(8))) //
			);

			//AddObject(new SpriteObject());
			AddObject(new EmptyGameObject().AddScript((scene, o, gameTime) =>
			{
				/* Close Game with Settings.ExitKey
				 * tempary Change ExitKey to P (didn't call SettingsManager.SaveSetting() so it will NOT be permanent, just for the time the application is running)
				 */
				if (KeyboardManager.IsKeyPressed(Settings.ExitKey))
					SceneManager.CloseScene();
				if (KeyboardManager.IsKeyPressed(Keys.I))
					SettingsManager.SetSetting("exitKey", Keys.P);

				if (KeyboardManager.IsKeyPressed(Keys.NumPad1))
				{
					// create a plane that spans over the x axis

					// p: x = ZERO3 + (0, 1, 0) + (0, 0, 1)

					// create the plane
					Vector3 planeOrigin = new Vector3(0, 0, 0);
					Vector3 planeParameter1 = new Vector3(1, 0, 1);
					Vector3 planeParameter2 = new Vector3(0, 1, 1);
					Vector3 planeNormal = new Vector3(0, 1, 0); // pp1 x pp2

					// create our point of interest
					Vector3 sphere = new Vector3(2, 3, 5);

					// We want to solve Ax = b
					// create our Vector "b", which is our solution
					Vector3 b = sphere - planeOrigin;

					// now we can create a room with our plane and the normal which contains all points in R3
					Vector3 solution = VectorMathHelper.SolveLinearEquation(planeParameter1, planeParameter2, planeNormal, b);

					// now we should have a solution. (-1, 0, 0)

					Console.WriteLine($"Solved {b} = {solution.X} * {planeParameter1} + {solution.Y} * {planeParameter2} + {solution.Z} * {planeNormal}");
					Console.WriteLine($"Calculated: {solution.X * planeParameter1 + solution.Y * planeParameter2 + solution.Z * planeNormal}");
					//Console.WriteLine($"Solution: {solution}");
					//Console.WriteLine(planeOrigin + solution.X * planeParameter1 + solution.Y * planeParameter2 + solution.Z * planeNormal);
				}

			}));

			AddObject(new EmptyGameObject().SetPosition(0, 10, 0).AddScript(
				((scene, o, arg3) => o.AddRotation(0, 1, 0, arg3))
			).AddChild(new CollidableModelObject("sphere").SetPosition(5, 0, 0), ChildProperties.TranslationRotation));


			// relative movement child test
			//AddObject(new CollidableModelObject("cubes/cube2")
			//	.SetPosition(20, 0, 0)
			//	.AddScript((GameScene scene, GameObject obj, GameTime time) =>
			//	{
			//		if (KeyboardManager.IsKeyPressed(Keys.NumPad2))
			//			Console.WriteLine($"True");

			//		if (KeyboardManager.IsKeyPressed(Keys.NumPad2))
			//		{
			//			if (obj.ParentObject == null)
			//			{
			//				// Add it
			//				// get player
			//				BasicCamera player = (BasicCamera)scene.GetAllObjects((GameObject o) => o is BasicCamera).First();
			//				player.AddChild(obj, ChildProperties.All | ChildProperties.KeepPositon);

			//			}
			//			else
			//			{
			//				// Remove it
			//				obj.RemoveParent();

			//			}
			//		}

			//	})
			//);

			AddObject(new EmptyGameObject().AddScript((GameScene scene, GameObject obj, GameTime time) =>
			{
				if (!obj.CustomData.ContainsKey("timeAlive"))
				{
					obj.CustomData.SetValue("timeAlive", 0.0f);
				}

				obj.CustomData.SetValue("timeAlive", obj.CustomData.GetValue<float>("timeAlive") + (float)time.ElapsedGameTime.TotalSeconds);


			}));

			AddObject(new CollidableModelObject("cubes/cube1")
				.SetCollision(new BoxCollision(new Vector3(-0.5f), new Vector3(0.5f)))
				.SetScale(8.0f)
				.SetPosition(50, 0, 0)

			);

			// Add Child Test
			AddObject(new ParentBlock().SetPosition(20, -10, 0).SetDebugName("Parent"));
			AddObject(new ChildBall().SetPosition(20, 00, 0).SetDebugName("Child"));


		}

		//public override void AddLightningToEffect(Effect eff)
		//{
		//	//var effect = (BasicEffect) eff;

		//	//effect.DirectionalLight0.DiffuseColor = new Vector3(0.15f, 0.15f, 0.15f); // some diffuse light
		//	//effect.DirectionalLight0.Direction = new Vector3(1, -2, 1);  // 
		//	//effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
		//	////effect.AmbientLightColor = new Vector3(1f, 1f, 1f); // Add some overall ambient light.
		//	//effect.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.315f); // Add some overall ambient light.
		//}
	}
}
