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
		public Func<float, float, Boolean> Restriction { get; private set; }

		public BoundEdgeCollision(GameObject parent, Vector3 origin, Vector3 spanVector, Func<float, float, Boolean> restriction) : this(parent, origin, spanVector, 0.0f, restriction)
		{}

		public BoundEdgeCollision(GameObject parent, Vector3 origin, Vector3 spanVector, float distance, Func<float, float, Boolean> restriction) : base(parent, origin, spanVector, distance)
		{
			this.Restriction = restriction;
		}
	}
}
