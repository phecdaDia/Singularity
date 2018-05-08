using System;
using Microsoft.Xna.Framework;
using Singularity.Code.Collisions;
using Singularity.Code.Collisions.CollisionTypes;
using Singularity.Collisions.CollisionTypes;

namespace Singularity.Collisions
{
	public static class CollisionManager
	{
		public static Boolean DoesCollide<T, U>(T collidableA, U collidableB, out Vector3 position, out Vector3 normal,
			Action<Collision, Collision, Vector3, Vector3> callback = null)
			where T : Collision
			where U : Collision
		{
			// default values for no collision
			position = new Vector3();
			normal = new Vector3(); // normal Vector of (0,0,0) should not be possible


			// check for the different types.
			// the normal vector should ALWAYS be from collidableA to collidableB

			var typeA = collidableA.GetType();
			var typeB = collidableB.GetType();

			// get the different collision types.
			var sphereCollision = typeof(SphereCollision);
			var planeCollision = typeof(PlaneCollision);
			var boundPlaneCollision = typeof(BoundPlaneCollision);
			var edgeCollision = typeof(EdgeCollision);
			var boundEdgeCollision = typeof(BoundEdgeCollision);

			var multiCollision = typeof(MultiCollision);

			if (typeA == sphereCollision && typeB == sphereCollision)
			{
				if (!SphereOnSphereCollision.GetCollision(collidableA as SphereCollision, collidableB as SphereCollision,
					out position, out normal)) return false;

				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (typeA == sphereCollision && typeB == planeCollision)
			{
				if (!SphereOnPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as PlaneCollision,
					out position, out normal)) return false;

				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == planeCollision && typeB == sphereCollision)
				return DoesCollide(collidableB, collidableA, out position, out normal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (typeA == sphereCollision && typeB == boundPlaneCollision)
			{
				if (!SphereOnBoundPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundPlaneCollision,
					out position, out normal)) return false;

				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == boundPlaneCollision && typeB == sphereCollision)
				return DoesCollide(collidableB, collidableA, out position, out normal);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (typeA == sphereCollision && typeB == edgeCollision)
			{
				if (!SphereOnEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as EdgeCollision,
					out position, out normal)) return false;

				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == edgeCollision && typeB == sphereCollision)
				return DoesCollide(collidableB, collidableA, out position, out normal);


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////


			else if (typeA == sphereCollision && typeB == boundEdgeCollision)
			{
				if (!SphereOnBoundEdgeCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundEdgeCollision,
					out position, out normal)) return false;

				callback?.Invoke(collidableA, collidableB, position, normal);
				return true;
			}
			else if (typeA == boundEdgeCollision && typeB == sphereCollision)
				return DoesCollide(collidableB, collidableA, out position, out normal);


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////

			else if (typeB == multiCollision || typeB.IsSubclassOf(multiCollision))
			{
				var didCollide = false;
				foreach (var coll in (collidableB as MultiCollision).GetCollidables())
				{
					if (!DoesCollide(collidableA, coll, out position, out normal)) continue;
					
					if (callback == null) return true;

					callback(collidableA, coll, position, normal);
					didCollide = true;
				}

				return didCollide;
			}
				
			else if (typeA == multiCollision || typeA.IsSubclassOf(multiCollision))
				return DoesCollide(collidableB, collidableA, out position, out normal, callback);

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

			// get the different collision types.
			var sphereCollision = typeof(SphereCollision);
			var planeCollision = typeof(PlaneCollision);
			var boundPlaneCollision = typeof(BoundPlaneCollision);
			var edgeCollision = typeof(EdgeCollision);
			var boundEdgeCollision = typeof(BoundEdgeCollision);

			if (typeA == sphereCollision && typeB == sphereCollision)
				return SphereOnSphereCollision.HandleCollision(collider as SphereCollision, collidable as SphereCollision, position, normal);

			else if (typeA == sphereCollision && typeB == planeCollision)
				return SphereOnPlaneCollision.HandleCollision(collider as SphereCollision, collidable as PlaneCollision, position, normal);

			else if (typeA == sphereCollision && typeB == boundPlaneCollision)
				return SphereOnBoundPlaneCollision.HandleCollision(collider as SphereCollision, collidable as BoundPlaneCollision, position, normal);

			else if (typeA == sphereCollision && typeB == edgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);

			else if (typeA == sphereCollision && typeB == boundEdgeCollision)
				return SphereOnEdgeCollision.HandleCollision(collider as SphereCollision, collidable as EdgeCollision, position, normal);



			return collider.Position;
		}
	}
}
