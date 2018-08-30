using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public class RingCollision : PlaneCollision
	{
		public RingCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2, float radius, float innerRadius) :
			base(origin, spanVector1, spanVector2)
		{
			this.Radius = radius;
			this.InnerRadius = innerRadius;
		}

		public float Radius { get; }
		public float InnerRadius { get; }

		public override object Clone()
		{
			return new RingCollision(this.Origin, this.SpanVector1, this.SpanVector2, this.Radius, this.InnerRadius);
		}
	}
}