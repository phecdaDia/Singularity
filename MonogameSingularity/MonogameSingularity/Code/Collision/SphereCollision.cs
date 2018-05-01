using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collision
{
	public class SphereCollision
	{
		public Vector3 Position { get; private set; }

		public float Radius { get; private set; }

		public SphereCollision(Vector3 position, float radius)
		{
			this.Position = position;
			this.Radius = radius;
		}

		public void SetPosition(Vector3 position)
		{
			this.Position = position;
		}

		public void SetRadius(float radius)
		{
			this.Radius = radius;
		}

	}
}
