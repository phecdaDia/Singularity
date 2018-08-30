using Microsoft.Xna.Framework;

namespace Singularity.Core.GameObjects
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