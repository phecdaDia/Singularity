using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class RingCollision : PlaneCollision
	{
		public RingCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2, float radius, float innerRadius) :
			base(origin, spanVector1, spanVector2)
		{
			Radius = radius;
			InnerRadius = innerRadius;
		}

		public float Radius { get; }
		public float InnerRadius { get; }

		public override object Clone()
		{
			return new RingCollision(Origin, SpanVector1, SpanVector2, Radius, InnerRadius);
		}
	}
}