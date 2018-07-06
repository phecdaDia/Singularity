namespace Singularity.Collisions
{
	public class SphereCollision : Collision
	{
		public SphereCollision(float radius = 1.0f)
		{
			Radius = radius;
		}

		public float Radius { get; }

		public override object Clone()
		{
			return new SphereCollision(Radius);
		}
	}
}