using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class RayCollisionPoint
	{
		public Vector3 Position { get; private set; }
		public Vector3 Normal { get; private set; }
		public Boolean DidCollide { get; private set; }
		public float RayDistance { get; private set; }

		public RayCollisionPoint(Vector3 position, Vector3 normal, float rayDistance)
		{
			Position = position;
			Normal = normal;
			DidCollide = true;
			RayDistance = rayDistance;
		}

		public RayCollisionPoint()
		{
			this.RayDistance = float.PositiveInfinity;
			DidCollide = false;
		}

		public void SetCollide(Boolean collide)
		{
			this.DidCollide = collide;
		}
	}
}
