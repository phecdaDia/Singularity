using Singularity.GameObjects;
using Singularity.GameObjects.Interfaces;
using Singularity.Utilities;

namespace SingularityTest.GameObjects
{
	public class CollidableTestObject : EmptyGameObject, ICollidable, IGlobal
	{
		public CollidableTestObject()
		{
			SetModel("cylinders/cylinder5");
			SetScale(1f, 1, 1f);


			//this.SetRotation(MathHelper.PiOver2, 0, 0);
			AddScript((scene, go, time) => AddRotation(0.4f, 0.6f, 1f, time));
		}
	}
}