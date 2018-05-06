using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions
{
	public class PlaneCollision : Collision
	{
		public Vector3 Origin { get; private set; }
		public Vector3 SpanVector1 { get; private set; }
		public Vector3 SpanVector2 { get; private set; }
		public Vector3 Normal { get; private set; }

		public PlaneCollision(GameObject parent, Vector3 origin, Vector3 spanVector1, Vector3 spanVector2) : base(parent)
		{
			Origin = origin;
			SpanVector1 = spanVector1;
			SpanVector2 = spanVector2;
			Normal = Vector3.Cross(spanVector1, spanVector2);
		}
		public PlaneCollision(GameObject parent, Vector3 origin, Vector3 spanVector1, Vector3 spanVector2, Vector3 normal) : this(parent, origin, spanVector1, spanVector2)
		{
			Normal = normal;
		}
	}
}
