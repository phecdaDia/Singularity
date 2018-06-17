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

        private readonly string _start;
        private readonly int _entranceId;
	    public StartingScene(SingularityGame game, string start, int entranceID) : base(game, "startingScene|" + start + "|" + entranceID)
	    {
	        _start = start;
	        _entranceId = entranceID;
	    }

	    protected override void AddGameObjects(int entranceId)
	    {
			AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
			                                          {
			                                              if (LoadingScreenTemplate.DoneLoading)
			                                              {
			                                                  SceneManager.ChangeScene(_start, _entranceId);
			                                                  LoadingScreenTemplate.DoneLoading = false;
			                                              }
			                                          }));
	    }

	    public override void AddLightningToEffect(Effect effect)
	    {
	    }
	}
}
