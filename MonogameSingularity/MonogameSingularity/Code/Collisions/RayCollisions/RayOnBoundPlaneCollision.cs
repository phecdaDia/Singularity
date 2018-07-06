using Microsoft.Xna.Framework;

namespace Singularity.Collisions.RayCollisions
{
	internal static class RayOnBoundPlaneCollision
	{
		public static RayCollisionPoint GetCollision(Ray ray, BoundPlaneCollision plane)
		{
			var rcp = RayOnPlaneCollision.GetCollision(ray, plane, out var s1, out var s2);

			if (!rcp.DidCollide) return rcp;

			rcp.SetCollide(plane.Restriction(s1, s2));

			return rcp;
		}
	}
}