using System;
using Microsoft.Xna.Framework;
using Singularity.Utilities;

namespace Singularity.Collisions.CollisionTypes
{
	internal static class SphereOnPlaneCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, PlaneCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale1, out float scale2)
		{

			normal = collidableB.Normal;
			normal.Normalize();
			Vector3 eq = VectorMathHelper.SolveLinearEquation(normal, collidableB.SpanVector1, collidableB.SpanVector2,
				collidableA.Position - collidableB.Origin);

			// point of collision

			position = collidableB.Origin + eq.Y * collidableB.SpanVector1 + eq.Z * collidableB.SpanVector2;
			normal = eq.X < 0 ? -normal : normal;
			scale1 = eq.Y;
			scale2 = eq.Z;

			return Math.Abs(eq.X) < collidableA.Radius * collidableA.Parent.Scale.X;

		}

		public static Boolean GetCollision(SphereCollision collidableA, PlaneCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1, out var f2);
		}

		public static Vector3 HandleCollision(SphereCollision collider, PlaneCollision collidable, Vector3 position, Vector3 normal)
		{
			return position + normal * collider.Radius;
		}
	}
}
