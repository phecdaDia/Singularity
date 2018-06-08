using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;
using Singularity.Utilities;

namespace Singularity.Collisions.RayCollisions
{
	internal static class RayOnPlaneCollision
	{

		public static RayCollisionPoint GetCollision(Ray ray, PlaneCollision plane, out float scale1, out float scale2)
		{
			try
			{

				// transformation matrix 2
				Vector3 eq = VectorMathHelper.SolveLinearEquation(-ray.Direction, plane.SpanVector1, plane.SpanVector2,
					ray.Position - plane.Origin);

				// point of collision
				Vector3 poc = ray.Position + eq.X * ray.Direction;

				scale1 = eq.Y;
				scale2 = eq.Z;

				return new RayCollisionPoint(poc, plane.Normal, eq.X);
			}
			catch (Exception ex)
			{
				// equation could not be solved or an error occured.

				Console.WriteLine($"Unable to collide ray with plane.");
#if __DEBUG__
				Console.WriteLine($"Ray: {ray.Position} + t * {ray.Direction}");
				Console.WriteLine($"Plane: {plane.Origin} + u * {plane.SpanVector1} + v * {plane.SpanVector2} (Normal: {plane.Normal})");
#endif
				scale1 = 0.0f;
				scale2 = 0.0f;

				return new RayCollisionPoint();
			}


		}

		public static RayCollisionPoint GetCollision(Ray ray, PlaneCollision plane)
		{
			return GetCollision(ray, plane, out float s1, out float s2);
		}
	}
}
