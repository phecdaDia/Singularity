using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Singularity;
using Singularity.Scripting;

using SingularityTest.GameObjects;

namespace SingularityTest
{
	public class LoadingScreen : LoadingScreenTemplate
	{
	    public LoadingScreen(SingularityGame game) : base(game)
	    {
	    }

	    protected override void AddGameObjects(int entranceId)
	    {
	        AddObject(new TestSpriteObject());
	    }

	    //public override void AddLightningToEffect(Effect effect)
	    //{
	        
	    //}
	}
}
