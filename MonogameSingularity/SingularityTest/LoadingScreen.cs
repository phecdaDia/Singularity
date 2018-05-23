using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Singularity;
using Singularity.Scripting;

using SingularityTest.GameObjects;

namespace SingularityTest
{
	/// <summary>
	/// Example Loading Screen -> just adds TestSpriteObject at 200, 200
	/// </summary>
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
