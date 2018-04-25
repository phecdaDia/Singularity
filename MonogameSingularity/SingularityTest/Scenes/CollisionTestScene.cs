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
		public override void AddLightningToEffect(BasicEffect effect)
		{
			effect.EnableDefaultLighting();
		}

		protected override void AddGameObjects()
		{
			AddCollider(new BasicCamera());

			for (int x = -10; x <= 10; x++)
			{
				AddCollider(new ModelObject("unit-cube-small").SetPosition(x, -2, -2));
			}


		}

		public CollisionTestScene() : base("collision", 10, -2, 0.1f)
		{

		}
	}
}
