using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Code.Collisions.Multi;
using Singularity.Collisions.Multi;
using Singularity.GameObjects;
using SingularityTest.GameObjects;
using SingularityTest.ScreenEffect;

namespace SingularityTest.Scenes
{
    public class TestScene : GameScene
	{
		public TestScene(SingularityGame game) : base(game, "test")
		{
			//this.SetCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

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
			} else if (entranceId == 1)
			{
				camPosition = new Vector3(10);
			}

			AddObject(new BasicCamera()
				.SetCameraTarget(camTarget)
				.SetPosition(camPosition)
				.AddScript((scene, obj, time) =>
				{
					// enable or disable 3d.
					if (KeyboardManager.IsKeyPressed(Keys.F1)) ((BasicCamera)obj).Set3DEnabled(!((BasicCamera)obj).Is3DEnabled);

					// some more movement options
					if (KeyboardManager.IsKeyDown(Keys.Q)) obj.AddPosition(new Vector3(0, 1, 0) * (float)time.ElapsedGameTime.TotalSeconds);
					if (KeyboardManager.IsKeyDown(Keys.E)) obj.AddPosition(new Vector3(0, -1, 0) * (float)time.ElapsedGameTime.TotalSeconds);

					// screen effects

					if (KeyboardManager.IsKeyPressed(Keys.Y))
						Game.ScreenEffectList.Add(ShakeScreenEffect.GetNewShakeScreenEffect(0.5f, 4).GetEffectData);
					if (KeyboardManager.IsKeyPressed(Keys.X))
						Game.ScreenEffectList.Add(ColorScreenEffect.GetNewColorScreenEffect(1, Color.Red).GetEffectData);
					if (KeyboardManager.IsKeyPressed(Keys.C))
						Game.SaveScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

					// savegame stuff
					sg.Position = obj.Position;
					sg.CameraTarget = ((BasicCamera)obj).GetCameraTarget();

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
				.SetCollision(new BoxCollision(new Vector3(-8), new Vector3(8)))
			);

			//AddObject(new SpriteObject());
			AddObject(new EmptyGameObject().AddScript((scene, o, gameTime) =>
		    {
				/* Close Game with Settings.ExitKey
				 * tempary Change ExitKey to P (didn't call SettingsManager.SaveSetting() so it will NOT be permanent, just for the time the application is running)
				 */
                if(KeyboardManager.IsKeyPressed(Settings.ExitKey))
                    SceneManager.CloseScene();
				if(KeyboardManager.IsKeyPressed(Keys.I))
					SettingsManager.SetSetting("exitKey", Keys.P);
		        if (KeyboardManager.IsKeyPressed(Keys.L))
		        {
		            SceneManager.ChangeScene("Scripts/CollisionTestSceneScript.csx");
		        }
		    }));


			// orbiting object.
			AddObject(new EmptyGameObject().SetPosition(0, 10, 0).AddScript(
				((scene, o, arg3) => o.AddRotation(0, (float) arg3.ElapsedGameTime.TotalSeconds, 0))
			).AddChild(new CollidableModelObject("sphere").SetPosition(5, 0, 0)));

			// inertia test
			//AddObject(new InertiaTestObject().SetPosition(-7.5f, 0, 0));

			AddObject(new TestSpriteObject().SetPosition(10, 10));

			//AddObject(new CollidableTestObject().SetPosition(10, 0, 0).SetCollision(new CylinderCollision(4.0f, 1.0f)));

			//AddObject(new CofTestObject().SetPosition(30, 0, 0));

		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			effect.DirectionalLight0.DiffuseColor = new Vector3(0.15f, 0.15f, 0.15f); // some diffuse light
			effect.DirectionalLight0.Direction = new Vector3(1, -2, 1);  // 
			effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
			//effect.AmbientLightColor = new Vector3(1f, 1f, 1f); // Add some overall ambient light.
			effect.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.315f); // Add some overall ambient light.
		}
	}
}
