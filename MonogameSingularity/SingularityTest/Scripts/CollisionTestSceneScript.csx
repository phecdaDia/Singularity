using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Collisions;
using Singularity.GameObjects;
using Singularity.Scripting;
using SingularityTest.GameObjects;

public class CollisionTestSceneScript : ScriptingTemplate
{
    public override SceneSettings GetSettings()
    {
        return new SceneSettings(){SceneSize = 10, MinPartition = -2, Precision = 0.1f};
    }

    public override List<GameObject> AddGameObjects(int entranceId)
    {
        var objList = new List<GameObject>();

		objList.Add(new EmptyGameObject().AddScript((scene, o, arg3) =>
		{
			if (KeyboardManager.IsKeyPressed(Keys.Escape))
				SceneManager.CloseScene();

			if (KeyboardManager.IsKeyPressed(Keys.F4))
				SceneManager.ClearStack();
		}));

        objList.Add(new StaticCamera().SetCameraTarget(new Vector3(0, 0, 1)).SetPosition(0, 1, -50)
								   .SetEnableCollision(false));


		// build the cube

		var planeCollision = new BoundPlaneCollision(new Vector3(), new Vector3(8, 0, 0), new Vector3(0, 0, 8),
													 (f1, f2) => -1 <= f1 && f1 <= 1 && -1 <= f2 && f2 <= 1);

        objList.Add(
				  new CollidableModelObject("planes/plane1")
					  .SetPosition(-8, 0, 0)
					  .SetCollision(planeCollision)
					  .SetRotation(0, 0, -MathHelper.PiOver2)
				 );

        objList.Add(
				  new CollidableModelObject("planes/plane1")
					  .SetPosition(8, 0, 0)
					  .SetCollision(planeCollision)
					  .SetRotation(0, 0, MathHelper.PiOver2)
				 );

        objList.Add(
				  new CollidableModelObject("planes/plane1")
					  .SetPosition(0, 8, 0)
					  .SetCollision(planeCollision)
					  .SetRotation(0, 0, MathHelper.Pi)
				 );

        objList.Add(
				  new CollidableModelObject("planes/plane1")
					  .SetPosition(0, -8, 0)
					  .SetCollision(planeCollision)
					  .SetRotation(0, 0, 0)
				 );

        objList.Add(
				  new CollidableModelObject("planes/plane1")
					  .SetPosition(0, 0, 8)
					  .SetCollision(planeCollision)
					  .SetRotation(-MathHelper.PiOver2, 0, 0)
				 );

        objList.Add(
				  new CollidableModelObject("planes/plane1")
					  .SetPosition(0, 0, -8)
					  .SetCollision(planeCollision)
					  .SetRotation(MathHelper.PiOver2, 0, 0)
				 );

		// add some objects to the cube


		// add balls

		Random random = new Random();

		for (int i = 0; i <= 5; i++)
		{
		    objList.Add(new TestBallObject()
					  .SetInertia((float)(5f * random.NextDouble()), 0, (float)(5f * random.NextDouble()))
					  .SetPosition((float)(5f * random.NextDouble()), (float)(5f * random.NextDouble()),
								   (float)(5f * random.NextDouble())));
		}

		//for (float x = 15; x >= -15; x -= 2.5f)
		//	AddObject(new TestBallObject().SetPosition(x, 10 + Math.Abs(x) / 2.0f, 0));

        return objList;
    }

	public override void AddLightningToEffect(Effect effect)
    {
		var eff = (BasicEffect)effect;
        eff.EnableDefaultLighting();
    }
}

return typeof(CollisionTestSceneScript);