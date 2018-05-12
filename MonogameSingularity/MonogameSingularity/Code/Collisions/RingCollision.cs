using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;

namespace Singularity.Code.Collisions
{
	public class RingCollision : PlaneCollision
	{

		public float Radius { get; private set; }
		public float InnerRadius { get; private set; }

		public RingCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2, float radius, float innerRadius) : base(origin, spanVector1, spanVector2)
		{
			Radius = radius;
			InnerRadius = innerRadius;
		}
	}
}
