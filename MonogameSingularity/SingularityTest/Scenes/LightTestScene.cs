using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity;
using Singularity.GameObjects;

namespace SingularityTest.Scenes
{
	public class LightTestScene : LightGameScene
	{

		public LightTestScene(SingularityGame game) : base(game, "light", 4096)
		{
			this.SetLightPosition(new Vector3(20, 20, 20));
			this.SetLightDirection(new Vector3(-1, -1, -1));

			this.SetProjectionMatrix(Matrix.CreateOrthographic(40, 40, 0.01f, 100.0f));

			this.SetAbsoluteCamera(new Vector3(-20, 20, 20), new Vector3(0, 0, 0));

		}

		protected override void AddGameObjects(int entranceId)
		{
			float width = 40.0f;
			
			int i = 0;
			
			while (width >= 1.0f)
			{
				AddObject(new ModelObject("cubes/cube1")
					.SetPosition(0, -0.5f + i, 0)
					.SetScale(width, 1, width)
				);
				
				i++;
				width /= 1.2f;
			}

			AddObject(new BasicCamera().Set3DEnabled(true).SetPosition(0, 10, 10));

		}
	}
}
