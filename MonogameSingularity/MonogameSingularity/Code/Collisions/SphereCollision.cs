namespace Singularity.Collisions
{
	public class SphereCollision : Collision
	{
		public float Radius { get; private set; }

		public SphereCollision() : base()
		{
			this.Radius = 1.0f; // default?
		}

		public SphereCollision(float radius) : base()
		{
			this.Radius = radius;
		}
	}
}
