using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class BoundEdgeCollision : EdgeCollision
	{
		public Func<float, float, Boolean> Restriction { get; private set; }

		public BoundEdgeCollision(Vector3 origin, Vector3 spanVector, Func<float, float, Boolean> restriction) : this(origin, spanVector, 0.0f, restriction)
		{}

		public BoundEdgeCollision(Vector3 origin, Vector3 spanVector, float distance, Func<float, float, Boolean> restriction) : base(origin, spanVector, distance)
		{
			this.Restriction = restriction;
		}

		public override object Clone()
		{
			return new BoundEdgeCollision(this.Origin, this.SpanVector, this.Distance, this.Restriction);
		}
	}
}
