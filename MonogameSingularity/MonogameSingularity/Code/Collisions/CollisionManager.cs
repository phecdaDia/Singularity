using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Code.Collisions.CollisionTypes;

namespace Singularity.Code.Collisions
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

			if (typeA == sphereCollision && typeB == sphereCollision)
				return SphereOnSphereCollision.GetCollision(collidableA as SphereCollision, collidableB as SphereCollision, out position, out normal);


			return false;
		}

		public static void HandleCollision(GameObject collider, GameObject collidable, Vector3 position, Vector3 normal)
		{
			// check for the different types.
			// the normal vector should ALWAYS be from collidableA to collidableB

			var typeA = collider.Collision.GetType();
			var typeB = collidable.Collision.GetType();

			// get the different collision types.
			var sphereCollision = typeof(SphereCollision);

			if (typeA == sphereCollision && typeB == sphereCollision)
				SphereOnSphereCollision.HandleCollision(collider, collidable, position, normal);


		}
	}
}
