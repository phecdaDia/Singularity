using System;
using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public abstract class Collision : ICloneable
	{
		public Collision(GameObject parent) : this()
		{
			this.Parent = parent;
		}

		public Collision()
		{
		}

		public Vector3 Position
		{
			get { return this.Parent.GetHierarchyPosition(); }
		}

		public GameObject Parent { get; private set; }

		public abstract object Clone();

		public virtual void SetParent(GameObject parent)
		{
			this.Parent = parent;
		}

		public virtual bool DoesCollide(Collision collidable, Action<Collision, Collision, Vector3, Vector3> callback = null,
			bool invertNormal = false)
		{
			return CollisionManager.DoesCollide(this, collidable, callback, invertNormal);
		}
	}
}