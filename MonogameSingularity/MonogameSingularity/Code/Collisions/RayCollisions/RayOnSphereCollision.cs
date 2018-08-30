using System;
using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions.RayCollisions
{
	internal static class RayOnSphereCollision
	{
		public static RayCollisionPoint GetCollision(Ray ray, SphereCollision sphere)
		{
			// https://gamedev.stackexchange.com/questions/96459/fast-ray-sphere-collision-code

			// distance from ray start to center of sphere
			var distance = ray.Position - sphere.Position;

			var radiusSquared = sphere.Radius * sphere.Radius;
			var pd = Vector3.Dot(distance, ray.Direction);

			if (pd > 0 || Vector3.Dot(distance, distance) < radiusSquared)
				return new RayCollisionPoint(); // no collision

			var a = distance - pd * ray.Direction;

			var aSquared = Vector3.Dot(a, a);

			if (aSquared > radiusSquared)
				return new RayCollisionPoint();

			var h = (float) Math.Sqrt(radiusSquared - aSquared);

			var i = a - h * ray.Direction;

			return new RayCollisionPoint(sphere.Parent, sphere.Position + i, i, h);
		}
	}
}