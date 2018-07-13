using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.GameObjects
{
	public class ModelObject : GameObject
	{
		public ModelObject(string modelPath) : base()
		{
			SetModel(modelPath);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
		}
	}
}