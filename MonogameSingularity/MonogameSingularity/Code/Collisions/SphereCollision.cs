namespace Singularity.Collisions
{
	public class SphereCollision : Collision
	{
		public float Radius { get; private set; }

		public SphereCollision(GameObject parent) : base(parent)
		{
			this.Radius = parent.ModelRadius;
		}

		public SphereCollision(GameObject parent, float radius) : base(parent)
		{
			this.Radius = radius;
		}
	}
}
