using Microsoft.Xna.Framework;
using Singularity.Core;

namespace Singularity.Examples.GameObjects
{
	public class ModelObject : GameObject
	{
		public ModelObject(string modelPath) : base()
		{
			this.SetModel(modelPath);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
		}
	}
}