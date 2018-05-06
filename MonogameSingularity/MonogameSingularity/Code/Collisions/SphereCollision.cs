using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions
{
	public class SphereCollision : Collision
	{
		public float Radius { get; private set; }

		public SphereCollision(GameObject parent) : base(parent)
		{
			this.Radius = parent.ModelRadius;
		}

		public SphereCollision(GameObject parent, float radius) : base(parent)
		{
			this.Radius = radius;
		}

		public override void ExitCollision(Vector3 position, Vector3 normal)
		{
			this.Parent.SetPosition(position + normal * Radius);
		}
	}
}
