namespace Singularity.Collisions
{
	public class SphereCollision : Collision
	{
		public float Radius { get; private set; }

		public SphereCollision(float radius = 1.0f) : base()
		{
			this.Radius = radius;
		}

		public override object Clone()
		{
			return new SphereCollision(this.Radius);
		}
	}
}
