using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code;
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
			AddObject(new BasicCamera(this));

			AddObject(new ModelObject("unit-cube").SetPosition(new Vector3(10, 0, 0)).SetScale(1f));
			AddObject(new ModelObject("unit-cube").SetPosition(new Vector3(10, 1, 0)).SetScale(1f));
			AddObject(new ModelObject("unit-cube").SetPosition(new Vector3(10, 2, 0)).SetScale(1f));
			AddObject(new ModelObject("unit-cube").SetPosition(new Vector3(10, 4, 0)).SetScale(1f));
			AddObject(new ModelObject("unit-cube").SetPosition(new Vector3(10, 5, 0)).SetScale(1f));

			AddObject(new ModelObject("unit-cube").SetPosition(new Vector3(2, 0, 2)).SetScale(1f));


			AddObject(new ModelObject("coin").SetPosition(new Vector3(-10, 0, 10)).SetScale(1f).SetRotation(new Vector3(0, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(-7.5f, 0, 5)).SetScale(1f).SetRotation(new Vector3(MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(-5f, 0, 10)).SetScale(1f).SetRotation(new Vector3(2 * MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(-2.5f, 0, 5)).SetScale(1f).SetRotation(new Vector3(3 * MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(0f, 0, 10)).SetScale(1f).SetRotation(new Vector3(4 * MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(2.5f, 0, 5)).SetScale(1f).SetRotation(new Vector3(5 * MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(5f, 0, 10)).SetScale(1f).SetRotation(new Vector3(6 * MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(7.5f, 0, 5)).SetScale(1f).SetRotation(new Vector3(7 * MathHelper.PiOver4, 0, 0)));
			AddObject(new ModelObject("coin").SetPosition(new Vector3(10f, 0, 10)).SetScale(1f).SetRotation(new Vector3(8 * MathHelper.PiOver4, 0, 0)));


			AddObject(new ModelObject("teapot").SetPosition(new Vector3(0, -10, 0)).SetScale(1f/25f).AddScript(
				new Action<GameScene, GameObject, GameTime>(
					(scene, obj, gameTime) =>
					{
						obj.AddRotation(new Vector3(0, 0, (float)gameTime.ElapsedGameTime.TotalSeconds));
					}
				)	
			));
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
