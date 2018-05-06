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
		public Vector3 SpanVectpr2 { get; private set; }

		public PlaneCollision(GameObject parent) : base(parent)
		{
		}
	}
}
