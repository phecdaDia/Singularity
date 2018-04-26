using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code;
using Singularity.Code.GameObjects;

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
			AddObject(new BasicCamera());

			for (int x = -10; x <= 10; x++)
			{
				AddObject(new ModelObject("unit-cube-small").SetPosition(x, -2, -2));
			}


		}
	}
}
