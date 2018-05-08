using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public abstract class Collision
	{
		public Vector3 Position
		{
			get { return Parent.GetHierarchyPosition(); }
		}

		public GameObject Parent { get; private set; }

		public Collision(GameObject parent)
		{
			this.Parent = parent;
		}

		public virtual Boolean DoesCollide(Collision collidable, out Vector3 position, out Vector3 normal)
		{
			return CollisionManager.DoesCollide(this, collidable, out position, out normal);
		}

	}
}
