using System.Collections.Generic;

namespace Singularity.Collisions
{
	public class MultiCollision : Collision
	{
		private readonly List<Collision> Collidables = new List<Collision>();

		public MultiCollision(params Collision[] collidables)
		{
			AddCollisions(collidables);
		}

		public void AddCollisions(params Collision[] collidables)
		{
			Collidables.AddRange(collidables);
		}

		public override void SetParent(GameObject parent)
		{
			base.SetParent(parent);

			foreach (var coll in Collidables) coll.SetParent(parent);
		}

		public override object Clone()
		{
			var _collidables = new List<Collision>();

			foreach (var coll in Collidables) _collidables.Add((Collision) coll.Clone());

			return new MultiCollision(_collidables.ToArray());
		}

		public List<Collision> GetCollidables()
		{
			return Collidables;
		}
	}
}