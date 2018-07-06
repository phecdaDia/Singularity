using System;
using Microsoft.Xna.Framework;
using Singularity.Collisions.CollisionTypes;
using Singularity.Collisions.RayCollisions;

namespace Singularity.Collisions
{
	public static class CollisionManager
	{
		public static Boolean DoesCollide<T, U>(T collidableA, U collidableB, 
			Action<Collision, Collision, Vector3, Vector3> callback = null, Boolean invertNormal = false)
			where T : Collision
			where U : Collision
		{
			// default values for no collision
			var position = new Vector3();
			var normal = new Vector3(); // normal Vector of (0,0,0) should not be possible

			var didCollide = false;

			#region Sphere On Collision


			#region Sphere on Sphere Collision

			if (collidableA is SphereCollision && collidableB is SphereCollision)
			{
				if (SphereOnSphereCollision.GetCollision(collidableA as SphereCollision, collidableB as SphereCollision, out position, out normal))
					didCollide = true;
			}

			#endregion

			#region Sphere on Bound Plane Collision

			else if (collidableA is SphereCollision && collidableB is BoundPlaneCollision)
			{ 
				if (SphereOnBoundPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundPlaneCollision, out position, out normal))
					didCollide = true;
			}
			else if (collidableA is BoundPlaneCollision && collidableB is SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			#endregion

			#region Sphere on Ring Collision


			else if (collidableA is SphereCollision && collidableB is RingCollision)
			{
				if (SphereOnRingCollision.GetCollision(collidableA as SphereCollision, collidableB as RingCollision, out position, out normal))
					didCollide = true;
			}
			else if (collidableA is RingCollision && collidableB is SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			#endregion

			#region Sphere on Plane Collision

			else if (collidableA is SphereCollision && collidableB is PlaneCollision)
			{
				if (SphereOnPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as PlaneCollision, out position, out normal))
					didCollide = true;
			}
			else if (collidableA is PlaneCollision && collidableB is SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			#endregion

			#region Sphere on Bound Edge Collision


			else if (collidableA is SphereCollision && collidableB is BoundEdgeCollision)
			{
				if (SphereOnBoundEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundEdgeCollision, out position, out normal))
					didCollide = true;
			}
			else if (collidableA is BoundEdgeCollision && collidableB is SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);


			#endregion

			#region Sphere on Edge Collision
			
			else if (collidableA is SphereCollision && collidableB is EdgeCollision)
			{
				if (SphereOnEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as EdgeCollision,out position, out normal))
					didCollide = true;

			}
			else if (collidableA is EdgeCollision && collidableB is SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);


			#endregion

			#endregion

			#region Multiple collision handlers Collision

			else if (collidableB is MultiCollision)
			{
				foreach (var coll in (collidableB as MultiCollision).GetCollidables())
				{
					if (DoesCollide(collidableA, coll, callback, invertNormal)) didCollide = true;
				}

				return didCollide;
			}
				
			else if (collidableA is MultiCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

		#endregion
			
			if (!didCollide) return false;

			if (invertNormal) normal = -normal;
			normal.Normalize();
			callback?.Invoke(collidableA, collidableB, position, normal);

			return true;

		}

		public static Vector3 HandleCollision<T, U>(T collider, U collidable, Vector3 position, Vector3 normal)
			where T : Collision
			where U : Collision
		{
			// check for the different types.
			// the normal vector should ALWAYS be from collidableA to collidableB

			if (collider is SphereCollision && collidable is SphereCollision)
				return SphereOnSphereCollision.HandleCollision(collider as SphereCollision, collidable as SphereCollision, position, normal);

			else if (collider is SphereCollision && collidable is BoundPlaneCollision)
				return SphereOnBoundPlaneCollision.HandleCollision(collider as SphereCollision, collidable as BoundPlaneCollision, position, normal);

			else if (collider is SphereCollision && collidable is RingCollision)
				return SphereOnRingCollision.HandleCollision(collider as SphereCollision, collidable as RingCollision, position, normal);

			else if (collider is SphereCollision && collidable is PlaneCollision)
				return SphereOnPlaneCollision.HandleCollision(collider as SphereCollision, collidable as PlaneCollision, position, normal);

			else if (collider is SphereCollision && collidable is BoundEdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);

			else if (collider is SphereCollision && collidable is EdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);



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

			if (collision is SphereCollision)
				return RayOnSphereCollision.GetCollision(ray, collision as SphereCollision); 
			else if (collision is BoundPlaneCollision)
				return RayOnBoundPlaneCollision.GetCollision(ray, collision as BoundPlaneCollision);
			else if (collision is PlaneCollision)
				return RayOnPlaneCollision.GetCollision(ray, collision as PlaneCollision);
			else if (collision is MultiCollision)
			{
				// we only want the closest RCP!
				RayCollisionPoint rcp = new RayCollisionPoint(); // dummy rcp
				MultiCollision mc = collision as MultiCollision;
				foreach (var coll in mc.GetCollidables())
				{
					RayCollisionPoint testPoint = GetRayCollision(ray, coll);
					if (testPoint.DidCollide && testPoint.RayDistance < rcp.RayDistance && testPoint.RayDistance >= 0) rcp = testPoint;
				}

				return rcp;
			}

			return new RayCollisionPoint();
		}
	}
}
