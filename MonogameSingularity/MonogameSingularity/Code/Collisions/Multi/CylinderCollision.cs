using System;
using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions.Multi
{
	public class CylinderCollision : MultiCollision
	{
		public CylinderCollision(float height, float radius)
		{
			Func<float, float, bool> circle = (f1, f2) => f1 * f1 + f2 * f2 <= radius * radius;

			this.AddCollisions(
				//new BoundEdgeCollision(this, new Vector3(0, -0.5f, 0), new Vector3(0, 1, 0), 1.0f, (scale, distance) => 0 <= scale && scale <= 1),
				//new BoundPlaneCollision(this, new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), (f1, f2) => f1 * f1 + f2 * f2 <= 1),
				//new BoundPlaneCollision(this, new Vector3(0, -0.5f, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), (f1, f2) => f1 * f1 + f2 * f2 <= 1),
				//new RingCollision(this, new Vector3(0.0f, 0.5f, 0.0f), new Vector3(1, 0, 0), new Vector3(0, 0, 1), 1.0f, 0.0f),
				//new RingCollision(this, new Vector3(0.0f, -0.5f, 0.0f), new Vector3(1, 0, 0), new Vector3(0, 0, 1), 1.0f, 0.0f)

				// add cylinder body
				new BoundEdgeCollision(new Vector3(0, -height / 2f, 0), new Vector3(0, height, 0), 1.0f,
					(scale, distance) => 0 <= scale && scale <= 1),

				// top plane
				new BoundPlaneCollision(new Vector3(0, height / 2f, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), circle),
				new BoundPlaneCollision(new Vector3(0, -height / 2f, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), circle),

				// seam edges
				new RingCollision(new Vector3(0.0f, height / 2f, 0.0f), new Vector3(1, 0, 0), new Vector3(0, 0, 1), radius, 0.0f),
				new RingCollision(new Vector3(0.0f, -height / 2f, 0.0f), new Vector3(1, 0, 0), new Vector3(0, 0, 1), radius, 0.0f)
			);
		}
	}
}