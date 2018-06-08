using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;
using Singularity.Utilities;

namespace Singularity.Collisions.RayCollisions
{
	internal static class RayOnBoundPlaneCollision
	{

		public static RayCollisionPoint GetCollision(Ray ray, BoundPlaneCollision plane)
		{
			RayCollisionPoint rcp = RayOnPlaneCollision.GetCollision(ray, plane, out float s1, out float s2);

			if (!rcp.DidCollide) return rcp;

			rcp.SetCollide(plane.Restriction(s1, s2));

			return rcp;

		}
	}
}
