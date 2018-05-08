using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;
using Singularity.Collisions.CollisionTypes;

namespace Singularity.Code.Collisions.CollisionTypes
{
	public static class SphereOnBoundEdgeCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, BoundEdgeCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale, out float distance)
		{

			if (SphereOnEdgeCollision.GetCollision(collidableA, collidableB, out position, out normal, out scale, out distance))
			{
				return collidableB.Restriction(scale, distance);
			}

			return false;


		}

		public static Boolean GetCollision(SphereCollision collidableA, BoundEdgeCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1, out var f2);
		}

		public static Vector3 HandleCollision(SphereCollision collider, EdgeCollision collidable, Vector3 position, Vector3 normal)
		{
			return position + normal * collider.Radius;
		}
	}
}
