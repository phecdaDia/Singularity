using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;

namespace Singularity.Code.Collisions.CollisionTypes
{
	public class SphereOnEdgeCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, EdgeCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale)
		{

			// transformation matrix 1
			var tm = collidableB.Parent.GetScaleMatrix * collidableB.Parent.GetRotationMatrix;

			var pos = collidableA.Position;
			var origin = collidableB.Position + Vector3.Transform(collidableB.Origin, tm);
			var sv = Vector3.Transform(collidableB.SpanVector, tm);
			scale = (sv.X * (pos.X - origin.X) + sv.Y * (pos.Y - origin.Y) + sv.Z * (pos.Z - origin.Z)) /
			        (sv.X * sv.X + sv.Y * sv.Y + sv.Z * sv.Z);

			//Console.WriteLine($"{scale}");

			position = origin + scale * sv;
			normal = pos - position;
			normal.Normalize();

			return (pos - position).Length() < collidableA.Radius;
		}

		public static Boolean GetCollision(SphereCollision collidableA, EdgeCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1);
		}

		public static Vector3 HandleCollision(SphereCollision collider, EdgeCollision collidable, Vector3 position, Vector3 normal)
		{
			return position + normal * collider.Radius;
		}
	}
}
