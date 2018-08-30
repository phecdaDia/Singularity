using System;
using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public class BoundEdgeCollision : EdgeCollision
	{
		public BoundEdgeCollision(Vector3 origin, Vector3 spanVector, Func<float, float, bool> restriction) : this(origin,
			spanVector, 0.0f, restriction)
		{
		}

		public BoundEdgeCollision(Vector3 origin, Vector3 spanVector, float distance, Func<float, float, bool> restriction) :
			base(origin, spanVector, distance)
		{
			this.Restriction = restriction;
		}

		public Func<float, float, bool> Restriction { get; }

		public override object Clone()
		{
			return new BoundEdgeCollision(this.Origin, this.SpanVector, this.Distance, this.Restriction);
		}
	}
}