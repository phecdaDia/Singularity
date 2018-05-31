using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions.CollisionTypes
{
	public static class SphereOnEdgeCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, EdgeCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale, out float distance)
		{

			// transformation matrix 1
			var tm = collidableB.Parent.ScaleMatrix * collidableB.Parent.RotationMatrix;

			var pos = collidableA.Position;
			var origin = collidableB.Position + Vector3.Transform(collidableB.Origin, tm);
			var sv = Vector3.Transform(collidableB.SpanVector, tm);
			scale = (sv.X * (pos.X - origin.X) + sv.Y * (pos.Y - origin.Y) + sv.Z * (pos.Z - origin.Z)) /
			        (sv.X * sv.X + sv.Y * sv.Y + sv.Z * sv.Z);

			//Console.WriteLine($"{scale}");

			position = origin + scale * sv;
			normal = pos - position;
			normal.Normalize();

			distance = (pos - position).Length();

			return distance < collidableB.Distance + collidableA.Radius;
		}

		public static Boolean GetCollision(SphereCollision collidableA, EdgeCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1, out var f2);
		}

		public static Vector3 HandleCollision(SphereCollision collider, EdgeCollision collidable, Vector3 position, Vector3 normal)
		{
			return position + normal * (collidable.Distance + collider.Radius);
		}
	}
}
