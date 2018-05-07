using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions
{
	public class BoundPlaneCollision : PlaneCollision
	{
		public Func<float, float, Boolean> Restriction { get; private set; }

		public BoundPlaneCollision(GameObject parent, Vector3 origin, Vector3 spanVector1, Vector3 spanVector2, Func<float, float, Boolean> restriction) : base(parent, origin, spanVector1, spanVector2)
		{
			this.Restriction = restriction;
		}
	}
}
