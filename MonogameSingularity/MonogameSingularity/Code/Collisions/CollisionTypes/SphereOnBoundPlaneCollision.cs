using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions.CollisionTypes
{
	internal static class SphereOnBoundPlaneCollision
	{
		public static bool GetCollision(SphereCollision collidableA, BoundPlaneCollision collidableB, out Vector3 position,
			out Vector3 normal, out float scale1, out float scale2)
		{
			if (SphereOnPlaneCollision.GetCollision(collidableA, collidableB, out position, out normal, out scale1, out scale2))
				return collidableB.Restriction(scale1, scale2);

			return false;
		}

		public static bool GetCollision(SphereCollision collidableA, BoundPlaneCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			return GetCollision(collidableA, collidableB, out position, out normal, out var f1, out var f2);
		}

		public static Vector3 HandleCollision(SphereCollision collider, BoundPlaneCollision collidable, Vector3 position,
			Vector3 normal)
		{
			return SphereOnPlaneCollision.HandleCollision(collider, collidable, position, normal);
		}
	}
}