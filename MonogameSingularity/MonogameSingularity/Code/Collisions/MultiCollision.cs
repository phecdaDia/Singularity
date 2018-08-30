using System.Collections.Generic;

namespace Singularity.Core.Collisions
{
	public class MultiCollision : Collision
	{
		private readonly List<Collision> Collidables = new List<Collision>();

		public MultiCollision(params Collision[] collidables)
		{
			this.AddCollisions(collidables);
		}

		public void AddCollisions(params Collision[] collidables)
		{
			this.Collidables.AddRange(collidables);
		}

		public override void SetParent(GameObject parent)
		{
			base.SetParent(parent);

			foreach (var coll in this.Collidables) coll.SetParent(parent);
		}

		public override object Clone()
		{
			var _collidables = new List<Collision>();

			foreach (var coll in this.Collidables) _collidables.Add((Collision) coll.Clone());

			return new MultiCollision(_collidables.ToArray());
		}

		public List<Collision> GetCollidables()
		{
			return this.Collidables;
		}
	}
}