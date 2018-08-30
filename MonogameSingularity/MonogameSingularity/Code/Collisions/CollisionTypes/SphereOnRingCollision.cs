using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions.CollisionTypes
{
	internal static class SphereOnRingCollision
	{
		public static bool GetCollision(SphereCollision collidableA, RingCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			// first let's get the collision parameters from the Plane Collision.
			SphereOnPlaneCollision.GetCollision(collidableA, collidableB, out var planeCollision, out var planeNormal,
				out var scale1, out var scale2);

			// Now get distance from our origin to the point planeCollision

			// transformation matrix 1
			var tm1 = collidableA.Parent.ScaleMatrix * collidableA.Parent.RotationMatrix;

			// transformation matrix 2
			var tm2 = collidableB.Parent.ScaleMatrix * collidableB.Parent.RotationMatrix;

			var sv1 = Vector3.Transform(collidableB.SpanVector1, tm2);
			var sv2 = Vector3.Transform(collidableB.SpanVector2, tm2);

			var distanceOnPlane = (scale1 * sv1 + scale2 * sv2).Length();

			var ringNorm = (scale1 * scale1 + scale2 * scale2) * collidableB.Radius;

			var ringScale1 = scale1 / ringNorm;
			var ringScale2 = scale2 / ringNorm;

			position = collidableB.Position + Vector3.Transform(collidableB.Origin, tm2) + ringScale1 * sv1 + ringScale2 * sv2;

			normal = collidableA.Position - position;
			var collide = normal.Length() < collidableA.Radius + collidableB.InnerRadius;

			normal.Normalize();

			return collide;
		}

		public static Vector3 HandleCollision(SphereCollision collider, RingCollision collidable, Vector3 position,
			Vector3 normal)
		{
			return position + normal * (collider.Radius + collidable.InnerRadius);
		}
	}
}