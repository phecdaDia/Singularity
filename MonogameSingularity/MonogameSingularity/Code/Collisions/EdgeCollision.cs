using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;

namespace Singularity.Code.Collisions
{
	public class EdgeCollision : Collision
	{
		public Vector3 Origin { get; private set; }

		public Vector3 SpanVector { get; private set; }

		public EdgeCollision(GameObject parent, Vector3 origin, Vector3 spanVector) : base(parent)
		{
			this.Origin = origin;
			this.SpanVector = spanVector;
		}
	}
}
