using System;
using Microsoft.Xna.Framework;
using Singularity.Code.Collisions;
using Singularity.Code.Collisions.CollisionTypes;
using Singularity.Collisions.CollisionTypes;

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


			// check for the different types.
			// the normal vector should ALWAYS be from collidableA to collidableB

			var typeA = collidableA.GetType();
			var typeB = collidableB.GetType();


			#region Sphere On Collision

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Sphere Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			if (typeA == SphereCollision && typeB == SphereCollision)
			{
				if (!SphereOnSphereCollision.GetCollision(collidableA as SphereCollision, collidableB as SphereCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Plane Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (typeA == SphereCollision && typeB == PlaneCollision)
			{
				if (!SphereOnPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as PlaneCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == PlaneCollision && typeB == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Bound Plane Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (typeA == SphereCollision && typeB == BoundPlaneCollision)
			{
				if (!SphereOnBoundPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundPlaneCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == BoundPlaneCollision && typeB == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Edge Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (typeA == SphereCollision && typeB == EdgeCollision)
			{
				if (!SphereOnEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as EdgeCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == EdgeCollision && typeB == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Bound Edge Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (typeA == SphereCollision && typeB == BoundEdgeCollision)
			{
				if (!SphereOnBoundEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundEdgeCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == BoundEdgeCollision && typeB == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Sphere on Ring Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (typeA == SphereCollision && typeB == RingCollision)
			{
				if (!SphereOnRingCollision.GetCollision(collidableA as SphereCollision, collidableB as RingCollision,
					out position, out normal)) return false;

				if (invertNormal) normal = -normal;
				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == RingCollision && typeB == SphereCollision)
				return DoesCollide(collidableB, collidableA, callback, !invertNormal);

			#endregion


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Multiple collision handlers Collision
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (typeB == MultiCollision || typeB.IsSubclassOf(MultiCollision))
			{
				var didCollide = false;
				foreach (var coll in (collidableB as MultiCollision).GetCollidables())
				{
					if (DoesCollide(collidableA, coll, callback, invertNormal)) didCollide = true;
				}

				return didCollide;
			}
				
			else if (typeA == MultiCollision || typeA.IsSubclassOf(MultiCollision))
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

			var typeA = collider.GetType();
			var typeB = collidable.GetType();

			if (typeA == SphereCollision && typeB == SphereCollision)
				return SphereOnSphereCollision.HandleCollision(collider as SphereCollision, collidable as SphereCollision, position, normal);

			else if (typeA == SphereCollision && typeB == PlaneCollision)
				return SphereOnPlaneCollision.HandleCollision(collider as SphereCollision, collidable as PlaneCollision, position, normal);

			else if (typeA == SphereCollision && typeB == BoundPlaneCollision)
				return SphereOnBoundPlaneCollision.HandleCollision(collider as SphereCollision, collidable as BoundPlaneCollision, position, normal);

			else if (typeA == SphereCollision && typeB == EdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);

			else if (typeA == SphereCollision && typeB == BoundEdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);

			else if (typeA == SphereCollision && typeB == RingCollision)
				return SphereOnRingCollision.HandleCollision(collider as SphereCollision, collidable as RingCollision, position, normal);



			return collider.Position;
		}
	}
}
