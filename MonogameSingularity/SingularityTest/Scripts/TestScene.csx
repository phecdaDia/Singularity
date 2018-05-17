using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SingularityTest.GameObjects;
using SingularityTest.ScreenEffect;

using Singularity;
using Singularity.Collisions.Multi;
using Singularity.GameObjects;

public class TestSceneScript : Singularity.GameScene
{
	public TestSceneScript(SingularityGame game) : base(game, "test")
	{
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

			if (KeyboardManager.IsKeyPressed(Keys.Y))
				Game.ScreenEffectList.Add(ShakeScreenEffect.GetNewShakeScreenEffect(0.5f, 4).GetEffectData);
			if (KeyboardManager.IsKeyPressed(Keys.X))
				Game.ScreenEffectList.Add(ColorScreenEffect.GetNewColorScreenEffect(1, Color.Red).GetEffectData);
			if (KeyboardManager.IsKeyPressed(Keys.C))
				Game.SaveScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
		}));

		AddObject(new ModelObject("slopes/slope1").SetPosition(-1.5f, -0.85f, -4));
		AddObject(new ModelObject("slopes/slope2").SetPosition(-0.5f, -0.85f, -2));
		AddObject(new ModelObject("slopes/slope3").SetPosition(0, -0.85f, 0));
		AddObject(new ModelObject("slopes/slope4").SetPosition(0, -0.35f, 2));
		AddObject(new ModelObject("slopes/slope5").SetPosition(0, 0.65f, 4));
		
		AddObject(new CollidableModelObject("cubes/cube5").SetPosition(0, -9, 0)
			.SetCollision(new BoxCollision(new Vector3(-8), new Vector3(8)))
		);

		AddObject(new EmptyGameObject().AddScript((scene, o, gameTime) =>
		{
			if (KeyboardManager.IsKeyPressed(Keys.Escape))
				Game.Exit();
		}));

		AddObject(new EmptyGameObject().SetPosition(0, 10, 0).AddScript(
			((scene, o, arg3) => o.AddRotation(0, (float)arg3.ElapsedGameTime.TotalSeconds, 0))
		).AddChild(new ModelObject("sphere").SetPosition(5, 0, 0)));

		AddObject(new TestSpriteObject().SetPosition(10, 10));
	}

	public override void AddLightningToEffect(BasicEffect effect)
	{
		effect.DirectionalLight0.DiffuseColor = new Vector3(0.15f, 0.15f, 0.15f);
		effect.DirectionalLight0.Direction = new Vector3(1, -2, 1); 
		effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f);
		effect.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.315f);
	}
}

return typeof(TestSceneScript);