using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.GameObjects
{
	public class ModelObject : GameObject
	{
		public ModelObject(String modelPath) : this(ModelManager.GetModel(modelPath))
		{
			this.SetTexture(ModelManager.GetTexture(modelPath));


		}
		public ModelObject(Model model) : base()
		{
			this.SetModel(model);
			
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{}
	}
}
