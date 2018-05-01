using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Code.Collision.Types
{
	public class SphereOnSphereCollision
	{

		public static Boolean DoesCollide(SphereCollision colliderA, SphereCollision colliderB)
		{
			// spheres collide if the distance between them is <= their radius added
			return (colliderA.Position - colliderB.Position).Length() <= colliderA.Radius + colliderB.Radius;
		}

	}
}
