using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class EdgeCollision : Collision
	{
		public EdgeCollision(Vector3 origin, Vector3 spanVector, float distance = 0.0f)
		{
			Origin = origin;
			SpanVector = spanVector;
			Distance = distance;
		}

		public Vector3 Origin { get; }

		public Vector3 SpanVector { get; }

		public float Distance { get; }

		public override object Clone()
		{
			return new EdgeCollision(Origin, SpanVector, Distance);
		}
	}
}