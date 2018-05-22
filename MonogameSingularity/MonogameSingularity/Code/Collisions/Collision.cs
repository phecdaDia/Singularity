using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public abstract class Collision : ICloneable
	{
		public Vector3 Position
		{
			get { return Parent.GetHierarchyPosition(); }
		}

		public GameObject Parent { get; private set; }

		public Collision(GameObject parent) : this()
		{
			this.Parent = parent;
		}
		public Collision()
		{}

		public virtual void SetParent(GameObject parent)
		{
			this.Parent = parent;
		}

		public virtual Boolean DoesCollide(Collision collidable, Action<Collision, Collision, Vector3, Vector3> callback = null, Boolean invertNormal = false)
		{
			return CollisionManager.DoesCollide(this, collidable, callback, invertNormal);
		}

		public abstract object Clone();
	}
}
