using System;
using Microsoft.Xna.Framework;
using Singularity.Core.Utilities;

namespace Singularity.Core.Collisions.RayCollisions
{
	internal static class RayOnPlaneCollision
	{
		public static RayCollisionPoint GetCollision(Ray ray, PlaneCollision plane, out float scale1, out float scale2)
		{
			try
			{
				// transformation matrix 2
				var eq = VectorMathHelper.SolveLinearEquation(-ray.Direction, plane.SpanVector1, plane.SpanVector2,
					ray.Position - plane.Origin);

				// point of collision
				var poc = ray.Position + eq.X * ray.Direction;

				scale1 = eq.Y;
				scale2 = eq.Z;


				return new RayCollisionPoint(plane.Parent, poc, plane.Normal, eq.X);
			}
			catch (Exception ex)
			{
				// equation could not be solved or an error occured.

				//Console.WriteLine($"Unable to collide ray with plane.");
				scale1 = 0.0f;
				scale2 = 0.0f;

				return new RayCollisionPoint();
			}
		}

		public static RayCollisionPoint GetCollision(Ray ray, PlaneCollision plane)
		{
			return GetCollision(ray, plane, out var s1, out var s2);
		}
	}
}