using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Scripting
{
    using GameObjects;
    using Microsoft.Xna.Framework.Graphics;

    internal class StartingScene : GameScene
    {

        private string _start;
	    public StartingScene(SingularityGame game, string start) : base(game, "startingScene")
	    {
	        _start = start;
	    }

	    protected override void AddGameObjects(int entranceId)
	    {
			AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
			                                          {
			                                              if (LoadingScreenTemplate.DoneLoading)
			                                              {
			                                                  SceneManager.ChangeScene(_start, entranceId, true);
			                                                  LoadingScreenTemplate.DoneLoading = false;
			                                              }
			                                          }));
	    }

	    public override void AddLightningToEffect(BasicEffect effect)
	    {
	    }
	}
}
