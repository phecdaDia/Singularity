using System;
using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public class BoundPlaneCollision : PlaneCollision
	{
		public BoundPlaneCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2,
			Func<float, float, bool> restriction) : base(origin, spanVector1, spanVector2)
		{
			this.Restriction = restriction;
		}

		public Func<float, float, bool> Restriction { get; }

		public override object Clone()
		{
			return new BoundPlaneCollision(this._origin, this._spanVector1, this._spanVector2, this.Restriction);
		}
	}
}