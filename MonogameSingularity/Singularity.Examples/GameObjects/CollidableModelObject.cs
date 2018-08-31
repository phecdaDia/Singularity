using Singularity.Core.GameObjects;

namespace Singularity.Examples.GameObjects
{
	public class CollidableModelObject : ModelObject, ICollidable
	{
		public CollidableModelObject(string modelPath) : base(modelPath)
		{
		}
	}
}