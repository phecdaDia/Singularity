using System;
using Microsoft.Xna.Framework;
using Singularity.Collisions.CollisionTypes;

namespace Singularity.Collisions
{
	public static class CollisionManager
	{
		public static Boolean DoesCollide<T, U>(T collidableA, U collidableB, out Vector3 position, out Vector3 normal)
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

			if (typeA == sphereCollision && typeB == sphereCollision)
				return SphereOnSphereCollision.GetCollision(collidableA as SphereCollision, collidableB as SphereCollision, out position, out normal);

			else if (typeA == sphereCollision && typeB == planeCollision)
				return SphereOnPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as PlaneCollision, out position, out normal);
			else if (typeA == planeCollision && typeB == sphereCollision)
				return SphereOnPlaneCollision.GetCollision(collidableB as SphereCollision, collidableA as PlaneCollision, out position, out normal);

			else if (typeA == sphereCollision && typeB == boundPlaneCollision)
				return SphereOnBoundPlaneCollision.GetCollision(collidableA as SphereCollision, collidableB as BoundPlaneCollision, out position, out normal);
			else if (typeA == boundPlaneCollision && typeB == sphereCollision)
				return SphereOnBoundPlaneCollision.GetCollision(collidableB as SphereCollision, collidableA as BoundPlaneCollision, out position, out normal);


			return false;
		}

		public static Vector3 HandleCollision(GameObject collider, GameObject collidable, Vector3 position, Vector3 normal)
		{
			// check for the different types.
			// the normal vector should ALWAYS be from collidableA to collidableB

			var typeA = collider.Collision.GetType();
			var typeB = collidable.Collision.GetType();

			// get the different collision types.
			var sphereCollision = typeof(SphereCollision);
			var planeCollision = typeof(PlaneCollision);
			var boundPlaneCollision = typeof(BoundPlaneCollision);

			if (typeA == sphereCollision && typeB == sphereCollision)
				return SphereOnSphereCollision.HandleCollision(collider, collidable, position, normal);

			else if (typeA == sphereCollision && typeB == planeCollision)
				return SphereOnPlaneCollision.HandleCollision(collider, collidable, position, normal);

			else if (typeA == sphereCollision && typeB == boundPlaneCollision)
				return SphereOnBoundPlaneCollision.HandleCollision(collider, collidable, position, normal);



			return collider.Position;
		}
	}
}
