using Microsoft.Xna.Framework.Graphics;
using Singularity.GameObjects.Interfaces;

namespace Singularity.GameObjects
{
	public class CollidableModelObject : ModelObject, ICollidable
	{
		public CollidableModelObject(string modelPath) : base(modelPath)
		{
		}

		public CollidableModelObject(Model model) : base(model)
		{
		}
	}
}