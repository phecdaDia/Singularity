using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions.CollisionTypes
{
	public static class SphereOnPlaneCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, PlaneCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale1, out float scale2)
		{

			// transformation matrix 1
			var tm1 = collidableA.Parent.ScaleMatrix * collidableA.Parent.RotationMatrix;

			// transformation matrix 2
			var tm2 = collidableB.Parent.ScaleMatrix * collidableB.Parent.RotationMatrix;

			normal = Vector3.Transform(collidableB.Normal, tm2);
			normal.Normalize();

			scale2 = 0.0f;
			scale1 = 0.0f;


			var sv1 = Vector3.Transform(collidableB.SpanVector1, tm2);
			var sv2 = Vector3.Transform(collidableB.SpanVector2, tm2);

			Matrix L = new Matrix(
				sv1.X, sv2.X, normal.X, 0,
				sv1.Y, sv2.Y, normal.Y, 0,
				sv1.Z, sv2.Z, normal.Z, 0,
				0, 0, 0, 1
			);

			//Console.WriteLine($"Matrix: {L}");

			var point = collidableA.Position;
			var origin = collidableB.Position + Vector3.Transform(collidableB.Origin, tm2);

			Vector4 b = new Vector4(
				point.X - origin.X,
				point.Y - origin.Y,
				point.Z - origin.Z,
				1
			);

			//Console.WriteLine($"B: {b}");

			Vector4 x = Vector4.Transform(b, L);

			scale1 = x.X / sv1.LengthSquared();
			scale2 = x.Y / sv2.LengthSquared();
			float t = x.Z;

			position = origin + scale1 * sv1 + scale2 * sv2;
			normal = (t < 0 ? -1 : 1) * normal;

			//Console.WriteLine("==========");
			//Console.WriteLine($"Position: {collidableA.Position} <=> {origin}, Distance: {t} * {normal}");
			//Console.WriteLine($"Scale1: {scale1} * {sv1}");
			//Console.WriteLine($"Scale2: {scale2} * {sv2}");
			//Console.WriteLine("==========");

			return Math.Abs(t) < collidableA.Radius * collidableA.Parent.Scale.X - 0.001f;

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
