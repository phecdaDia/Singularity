using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;

namespace Singularity.Collisions
{
	public class MultiCollision : Collision
	{
		private readonly List<Collision> Collidables = new List<Collision>();

		public MultiCollision(params Collision[] collidables) : base()
		{
			AddCollisions(collidables);
		}

		public void AddCollisions(params Collision[] collidables)
		{
			this.Collidables.AddRange(collidables);
		}

		public override void SetParent(GameObject parent)
		{
			base.SetParent(parent);

			foreach (var coll in Collidables)
			{
				coll.SetParent(parent);
			}
		}

		public List<Collision> GetCollidables()
		{
			return this.Collidables;
		}
	}
}
