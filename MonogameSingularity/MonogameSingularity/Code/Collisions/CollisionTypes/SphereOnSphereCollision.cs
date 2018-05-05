﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions.CollisionTypes
{
	public static class SphereOnSphereCollision
	{
		public static Boolean GetCollision(SphereCollision collidableA, SphereCollision collidableB, out Vector3 position,
			out Vector3 normal)
		{
			var radiusDist = collidableA.Radius * collidableA.Parent.Scale.X + collidableB.Radius * collidableB.Parent.Scale.X;
			var dist = collidableA.Position - collidableB.Position;

			if (dist.LengthSquared() >= radiusDist * radiusDist)
			{
				// no collision
				position = new Vector3();
				normal = new Vector3();
				return false;
			}

			// we got a collision. 
			// normal vector should always be from A to B, thus B collides with A and needs to exit.
			normal = dist;
			normal.Normalize();

			position = collidableB.Position + normal * collidableB.Radius * collidableB.Parent.Scale.X;

			return true;
		}

		public static void HandleCollision(GameObject collider, GameObject collidable, Vector3 position, Vector3 normal)
		{
			collider.SetPosition(position + normal * ((SphereCollision) collider.Collision).Radius);
		}
	}
}