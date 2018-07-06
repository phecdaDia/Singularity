using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class BoundPlaneCollision : PlaneCollision
	{
		public BoundPlaneCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2,
			Func<float, float, bool> restriction) : base(origin, spanVector1, spanVector2)
		{
			Restriction = restriction;
		}

		public Func<float, float, bool> Restriction { get; }

		public override object Clone()
		{
			return new BoundPlaneCollision(_origin, _spanVector1, _spanVector2, Restriction);
		}
	}
}