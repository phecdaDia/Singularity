using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions.CollisionTypes
{
	public static class SphereOnPlaneCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, PlaneCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale1, out float scale2)
		{

			// transformation matrix 1
			var tm1 = collidableA.Parent.GetScaleMatrix * collidableA.Parent.GetRotationMatrix;

			// transformation matrix 2
			var tm2 = collidableB.Parent.GetScaleMatrix * collidableB.Parent.GetRotationMatrix;

			normal = Vector3.Transform(collidableB.Normal, tm2);
			normal.Normalize();

			var p2 = collidableB.Parent.Position + Vector3.Transform(collidableB.Origin, tm2);

			var t = -normal.X * (collidableA.Position.X - p2.X)
			        - normal.Y * (collidableA.Position.Y - p2.Y)
			        - normal.Z * (collidableA.Position.Z - p2.Z);

			t = t / (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

			position = collidableA.Position + t * normal;

			if ((t * normal).LengthSquared() > collidableA.Radius * collidableA.Radius - 0.001f)
			{
				// no collision

				scale1 = 0.0f;
				scale2 = 0.0f;

				return false;
			}

			// calculate scale1 and 2

			scale2 = 0.0f;
			scale1 = 0.0f;

			var sv1 = Vector3.Transform(collidableB.SpanVector1, tm2);
			var sv2 = Vector3.Transform(collidableB.SpanVector2, tm2);

			// TODO calculate parameters

			return true;

		}

		public static Boolean GetCollision(SphereCollision collidableA, PlaneCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1, out var f2);
		}

		public static Vector3 HandleCollision(GameObject collider, GameObject collidable, Vector3 position, Vector3 normal)
		{
			return position + normal * ((SphereCollision) collider.Collision).Radius;
		}
	}
}
