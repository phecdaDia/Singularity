using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions.RayCollisions
{
	internal static class RayOnSphereCollision
	{
		public static RayCollisionPoint GetCollision(Ray ray, SphereCollision sphere)
		{
			// https://gamedev.stackexchange.com/questions/96459/fast-ray-sphere-collision-code

			// distance from ray start to center of sphere
			Vector3 distance = ray.Position - sphere.Position;

			float radiusSquared = sphere.Radius * sphere.Radius;
			float pd = Vector3.Dot(distance, ray.Direction);

			if (pd > 0 || Vector3.Dot(distance, distance) < radiusSquared)
				return new RayCollisionPoint(); // no collision

			Vector3 a = distance - pd * ray.Direction;

			float aSquared = Vector3.Dot(a, a);

			if (aSquared > radiusSquared)
				return new RayCollisionPoint();

			float h = (float) Math.Sqrt(radiusSquared - aSquared);

			Vector3 i = a - h * ray.Direction;

			return new RayCollisionPoint(sphere.Position + i, i, h);
		}
	}
}
