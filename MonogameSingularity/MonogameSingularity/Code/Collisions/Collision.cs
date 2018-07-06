using System;
using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public abstract class Collision : ICloneable
	{
		public Collision(GameObject parent) : this()
		{
			Parent = parent;
		}

		public Collision()
		{
		}

		public Vector3 Position
		{
			get { return Parent.GetHierarchyPosition(); }
		}

		public GameObject Parent { get; private set; }

		public abstract object Clone();

		public virtual void SetParent(GameObject parent)
		{
			Parent = parent;
		}

		public virtual bool DoesCollide(Collision collidable, Action<Collision, Collision, Vector3, Vector3> callback = null,
			bool invertNormal = false)
		{
			return CollisionManager.DoesCollide(this, collidable, callback, invertNormal);
		}
	}
}