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

			scale2 = 0.0f;
			scale1 = 0.0f;


			var sv1 = Vector3.Transform(collidableB.SpanVector1, tm2);
			var sv2 = Vector3.Transform(collidableB.SpanVector2, tm2);

			//Console.WriteLine($"DEFAULT: {collidableB.SpanVector1} x {collidableB.SpanVector2} => {collidableB.Normal}");
			//Console.WriteLine($"TRANSFO: {sv1} x {sv2} => {normal}");

			Matrix L = Matrix.Invert(new Matrix(
				sv1.X, sv2.X, normal.X, 0,
				sv1.Y, sv2.Y, normal.Y, 0,
				sv1.Z, sv2.Z, normal.Z, 0,
				0, 0, 0, 1
			));

			var point = collidableA.Position;
			var origin = collidableB.Position + Vector3.Transform(collidableB.Origin, tm2);

			Vector4 b = new Vector4(
				point.X - origin.X,
				point.Y - origin.Y,
				point.Z - origin.Z,
				1
			);
			
			Vector4 x = Vector4.Transform(b, L);

			//Console.WriteLine(x);
			scale1 = x.Z;
			scale2 = x.X;
			float t = x.Y;


			position = origin + scale1 * sv1 + scale2 * sv2;
			normal = (t < 0 ? -1 : 1) * normal;
			
			//Console.WriteLine($"{position} = {origin} + {scale1} * {sv1} + {scale2} * {sv2}");

			return Math.Abs(t) < collidableA.Radius * collidableA.Parent.Scale.X;

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
