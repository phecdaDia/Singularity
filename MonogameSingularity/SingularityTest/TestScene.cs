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
			AddObject(new BasicCamera(this).AddScript((scene, obj, time) =>
			{
				if (KeyboardManager.IsKeyDown(Keys.F1)) ((BasicCamera)obj).Set3DEnabled(!((BasicCamera)obj).Is3DEnabled);


				if (KeyboardManager.IsKeyPressed(Keys.Q)) obj.AddPosition(new Vector3(0, 0, 1) * (float)time.ElapsedGameTime.TotalSeconds);
				if (KeyboardManager.IsKeyPressed(Keys.E)) obj.AddPosition(new Vector3(0, 0, -1) * (float)time.ElapsedGameTime.TotalSeconds);
			}));

			AddObject(new ModelObject("sphere").SetPosition(new Vector3(5, 20, 0)));

			AddObject(new ModelObject("unit-cube-small").SetPosition(new Vector3(5, 3, 0)));


			AddObject(new ModelObject("wood_table").SetPosition(new Vector3(0, 0, -2f)));

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
