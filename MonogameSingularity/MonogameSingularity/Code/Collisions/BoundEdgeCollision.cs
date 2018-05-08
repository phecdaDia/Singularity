using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Collisions
{
	public class BoundEdgeCollision : EdgeCollision
	{
		public Func<float, Boolean> Restriction { get; private set; }

		public BoundEdgeCollision(GameObject parent, Vector3 origin, Vector3 spanVector, Func<float, Boolean> restriction) : base(parent, origin, spanVector)
		{
			this.Restriction = restriction;
		}
	}
}
