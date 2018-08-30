using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public class EdgeCollision : Collision
	{
		public EdgeCollision(Vector3 origin, Vector3 spanVector, float distance = 0.0f)
		{
			this.Origin = origin;
			this.SpanVector = spanVector;
			this.Distance = distance;
		}

		public Vector3 Origin { get; }

		public Vector3 SpanVector { get; }

		public float Distance { get; }

		public override object Clone()
		{
			return new EdgeCollision(this.Origin, this.SpanVector, this.Distance);
		}
	}
}