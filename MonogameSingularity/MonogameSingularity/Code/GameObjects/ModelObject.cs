using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.GameObjects
{
	public class ModelObject : GameObject
	{
		public ModelObject(string modelPath) : this(ModelManager.GetModel(modelPath))
		{
			SetTexture(ModelManager.GetTexture(modelPath));
		}

		public ModelObject(Model model)
		{
			SetModel(model);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
		}
	}
}