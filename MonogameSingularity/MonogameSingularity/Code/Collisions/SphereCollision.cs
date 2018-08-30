namespace Singularity.Core.Collisions
{
	public class SphereCollision : Collision
	{
		public SphereCollision(float radius = 1.0f)
		{
			this.Radius = radius;
		}

		public float Radius { get; }

		public override object Clone()
		{
			return new SphereCollision(this.Radius);
		}
	}
}