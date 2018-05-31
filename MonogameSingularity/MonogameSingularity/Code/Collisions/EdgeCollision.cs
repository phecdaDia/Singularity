using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class EdgeCollision : Collision
	{
		public Vector3 Origin { get; private set; }

		public Vector3 SpanVector { get; private set; }

		public float Distance { get; private set; }

		public EdgeCollision(Vector3 origin, Vector3 spanVector, float distance = 0.0f) : base()
		{
			this.Origin = origin;
			this.SpanVector = spanVector;
			this.Distance = distance;

		}

		public override object Clone()
		{
			return new EdgeCollision(this.Origin, this.SpanVector, this.Distance);
		}
	}
}
