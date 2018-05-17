using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using Singularity;
using Singularity.GameObjects;
using Singularity.Scripting;

using SingularityTest.GameObjects;

namespace SingularityTest
{
	public class LoadingScreen : LoadingScreenTemplate
	{
		public override void AddGameObjects(List<GameObject> objectList)
		{
			objectList.Add(new TestSpriteObject().SetPosition(200,200));
		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			
		}
	}
}
