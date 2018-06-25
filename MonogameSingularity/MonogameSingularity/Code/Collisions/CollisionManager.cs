using System;
using Microsoft.Xna.Framework;
using Singularity.Collisions.CollisionTypes;
using Singularity.Collisions.RayCollisions;

namespace Singularity.Collisions
{
	public static class CollisionManager
	{
		#region Collision Types
		private static readonly Type SphereCollision = typeof(SphereCollision);
		private static readonly Type PlaneCollision = typeof(PlaneCollision);
		private static readonly Type BoundPlaneCollision = typeof(BoundPlaneCollision);
		private static readonly Type EdgeCollision = typeof(EdgeCollision);
		private static readonly Type BoundEdgeCollision = typeof(BoundEdgeCollision);
		private static readonly Type RingCollision = typeof(RingCollision);

		private static readonly Type MultiCollision = typeof(MultiCollision);
		#endregion


		public static Boolean DoesCollide<T, U>(T collidableA, U collidableB, 
			Action<Collision, Collision, Vector3, Vector3> callback = null, Boolean invertNormal = false)
			where T : Collision
			where U : Collision
		{
			// default values for no collision
			var position = new Vector3();
			var normal = new Vector3(); // normal Vector of (0,0,0) should not be possible
			
			#region Sphere On Collision

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Sphere Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			if (collidableA.GetType() == SphereCollision && collidableB.GetType() == SphereCollision)
			{
				if (!SphereOnSphereCollision.GetCollision(collidableA as SphereCollision, collidableB as SphereCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				normal.Normalize();
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Plane Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (collidableA.GetType() == SphereCollision && collidableB.GetType() == PlaneCollision)
			{
				if (!SphereOnPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as PlaneCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				normal.Normalize();
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (collidableA.GetType() == PlaneCollision && collidableB.GetType() == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Bound Plane Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (collidableA.GetType() == SphereCollision && collidableB.GetType() == BoundPlaneCollision)
			{
				if (!SphereOnBoundPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundPlaneCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				normal.Normalize();
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (collidableA.GetType() == BoundPlaneCollision && collidableB.GetType() == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Edge Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (collidableA.GetType() == SphereCollision && collidableB.GetType() == EdgeCollision)
			{
				if (!SphereOnEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as EdgeCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				normal.Normalize();
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (collidableA.GetType() == EdgeCollision && collidableB.GetType() == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Bound Edge Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (collidableA.GetType() == SphereCollision && collidableB.GetType() == BoundEdgeCollision)
			{
				if (!SphereOnBoundEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundEdgeCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				normal.Normalize();
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (collidableA.GetType() == BoundEdgeCollision && collidableB.GetType() == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Ring Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (collidableA.GetType() == SphereCollision && collidableB.GetType() == RingCollision)
			{
				if (!SphereOnRingCollision.GetCollision(collidableA as SphereCollision, collidableB as RingCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				normal.Normalize();
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (collidableA.GetType() == RingCollision && collidableB.GetType() == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			#endregion


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Multiple collision handlers Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (collidableB.GetType() == MultiCollision || collidableB.GetType().IsSubclassOf(MultiCollision))
			{
				var didCollide = false;
				foreach (var coll in (collidableB as MultiCollision).GetCollidables())
				{
					if (DoesCollide(collidableA, coll, callback, invertNormal)) didCollide = true;
				}

				return didCollide;
			}
				
			else if (collidableA.GetType() == MultiCollision || collidableA.GetType().IsSubclassOf(MultiCollision))
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			return false;
		}

		public static Vector3 HandleCollision<T, U>(T collider, U collidable, Vector3 position, Vector3 normal)
			where T : Collision
			where U : Collision
		{
			// check for the different types.
			// the normal vector should ALWAYS be from collidableA to collidableB

			if (collider.GetType() == SphereCollision && collidable.GetType() == SphereCollision)
				return SphereOnSphereCollision.HandleCollision(collider as SphereCollision, collidable as SphereCollision, position, normal);

			else if (collider.GetType() == SphereCollision && collidable.GetType() == PlaneCollision)
				return SphereOnPlaneCollision.HandleCollision(collider as SphereCollision, collidable as PlaneCollision, position, normal);

			else if (collider.GetType() == SphereCollision && collidable.GetType() == BoundPlaneCollision)
				return SphereOnBoundPlaneCollision.HandleCollision(collider as SphereCollision, collidable as BoundPlaneCollision, position, normal);

			else if (collider.GetType() == SphereCollision && collidable.GetType() == EdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);

			else if (collider.GetType() == SphereCollision && collidable.GetType() == BoundEdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);

			else if (collider.GetType() == SphereCollision && collidable.GetType() == RingCollision)
				return SphereOnRingCollision.HandleCollision(collider as SphereCollision, collidable as RingCollision, position, normal);



			return collider.Position;
		}

		public static RayCollisionPoint GetRayCollision<T>(Ray ray, T collision)
			where T : Collision
		{
			//Console.WriteLine($"Testing RCP with {typeof(T)} (Ray: {ray.Position} + t * {ray.Direction})");

			//if (collision == null)
			//{
			//	// ???
			//	Console.WriteLine($"Received empty collision to check!");
			//	return new RayCollisionPoint();
			//}

			if (collision.GetType() == SphereCollision)
				return RayOnSphereCollision.GetCollision(ray, collision as SphereCollision); 
			else if (collision.GetType() == BoundPlaneCollision)
				return RayOnBoundPlaneCollision.GetCollision(ray, collision as BoundPlaneCollision);
			else if (collision.GetType() == PlaneCollision)
				return RayOnPlaneCollision.GetCollision(ray, collision as PlaneCollision);
			else if (collision.GetType() == MultiCollision)
			{
				// we only want the closest RCP!
				RayCollisionPoint rcp = new RayCollisionPoint(); // dummy rcp
				MultiCollision mc = collision as MultiCollision;
				foreach (var coll in mc.GetCollidables())
				{
					RayCollisionPoint testPoint = GetRayCollision(ray, coll);
					if (testPoint.DidCollide && testPoint.RayDistance < rcp.RayDistance) rcp = testPoint;
				}

				return rcp;
			}

			return new RayCollisionPoint();
		}
	}
}
