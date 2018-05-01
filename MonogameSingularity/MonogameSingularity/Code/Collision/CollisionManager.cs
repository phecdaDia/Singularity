using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Code.Collision.Types;

namespace Singularity.Code.Collision
{
	public class CollisionManager
	{

		public static Boolean DoesCollide<T, U>(T colliderA, U colliderB) where T : Collision where U : Collision
		{
			// get the types
			var typeA = typeof(T);
			var typeB = typeof(U);

			// now get all our types
			var sphereType = typeof(SphereCollision);


			// now we have 2^n if statements

			if (typeA == sphereType && typeB == sphereType)
				return SphereOnSphereCollision.DoesCollide(colliderA as SphereCollision, colliderB as SphereCollision);
			


			Console.Error.WriteLine($"Unknown collision behaviour {typeA} - {typeB}");
			return true;
		}
	}
}
