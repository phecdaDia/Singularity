using Singularity.Core.GameObjects.Interfaces;

namespace Singularity.Core.GameObjects
{
	public class CollidableModelObject : ModelObject, ICollidable
	{
		public CollidableModelObject(string modelPath) : base(modelPath)
		{
		}
	}
}