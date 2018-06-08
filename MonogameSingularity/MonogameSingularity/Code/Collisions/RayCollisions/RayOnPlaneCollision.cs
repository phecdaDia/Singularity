using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;
using Singularity.Utilities;

namespace Singularity.Collisions.RayCollisions
{
	internal class RayOnPlaneCollision
	{

		public static Boolean DoesCollide(Ray ray, PlaneCollision plane, out RayCollisionPoint collisionPoint)
		{
			try
			{

				// transformation matrix 2
				Vector3 eq = VectorMathHelper.SolveLinearEquation(-ray.Direction, plane.SpanVector1, plane.SpanVector2,
					ray.Position - plane.Origin);

				// point of collision
				Vector3 poc = ray.Position + eq.X * ray.Direction;
				collisionPoint = new RayCollisionPoint(poc, plane.Normal, eq.X);

				return true;
			}
			catch (Exception)
			{
				// equation could not be solved or an error occured.
				Console.WriteLine($"Unable to collide ray with plane.");
				
				collisionPoint = new RayCollisionPoint();
				return false;
			}


		}
	}
}
