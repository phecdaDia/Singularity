using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions.CollisionTypes
{
	public static class SphereOnBoundPlaneCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, BoundPlaneCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale1, out float scale2)
		{

			if (SphereOnPlaneCollision.GetCollision(collidableA, collidableB, out position, out normal, out scale1, out scale2))
			{
				Console.WriteLine($"COLL {scale1}, {scale2}");
				return collidableB.Restriction(scale1, scale2);
			}

			Console.WriteLine($"NCOL {scale1}, {scale2}");

			return false;


		}

		public static Boolean GetCollision(SphereCollision collidableA, BoundPlaneCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1, out var f2);
		}

		public static Vector3 HandleCollision(GameObject collider, GameObject collidable, Vector3 position, Vector3 normal)
		{
			return position + normal * ((SphereCollision)collider.Collision).Radius;
		}
	}
}
