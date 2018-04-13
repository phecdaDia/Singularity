using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code.GameObjects
{
	public class ModelObject : GameObject
	{
		public ModelObject(String modelPath)
		{
			this.SetModel(ModelManager.GetModel(modelPath));
		}
		public ModelObject(Model model)
		{
			this.SetModel(model);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			//Console.WriteLine(this.Position);

		} // No logic. It's only rendering the model.
	}
}
