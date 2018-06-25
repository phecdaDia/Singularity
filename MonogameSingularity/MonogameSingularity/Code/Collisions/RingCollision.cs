using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class RingCollision : PlaneCollision
	{

		public float Radius { get; private set; }
		public float InnerRadius { get; private set; }

		public RingCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2, float radius, float innerRadius) : base(origin, spanVector1, spanVector2)
		{
			Radius = radius;
			InnerRadius = innerRadius;
		}

		public override object Clone()
		{
			return new RingCollision(this.Origin, this.SpanVector1, this.SpanVector2, this.Radius, this.InnerRadius);
		}
	}
}
