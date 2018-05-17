using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Scripting
{
	public abstract class LoadingScreenTemplate
	{
		public SingularityGame Game { get; private set; }

		public virtual void Init(SingularityGame game)
		{
			Game = game;
		}
		public abstract void AddGameObjects(List<GameObject> objectList);
		public abstract void AddLightningToEffect(BasicEffect effect);
	}
}
