using Microsoft.Xna.Framework;

namespace Singularity.Collisions.Multi
{
	public class BoxCollision : MultiCollision
	{
		public BoxCollision(float min, float max) : this(new Vector3(min), new Vector3(max)) { }
		public BoxCollision(Vector3 min, Vector3 max)
		{
			bool _squareBoundary(float f1, float f2)
			{
				return 0 <= f1 && f1 <= 1 && 0 <= f2 && f2 <= 1;
			}

			bool _maxScale1(float scale, float distance)
			{
				return 0 <= scale && scale <= 1;
			}

			AddCollisions(
				new BoundPlaneCollision(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X - min.X, 0, 0),
					new Vector3(0, max.Y - min.Y, 0),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(min.X, min.Y, min.Z), new Vector3(0, max.Y - min.Y, 0),
					new Vector3(0, 0, max.Z - min.Z),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(min.X, min.Y, min.Z), new Vector3(0, 0, max.Z - min.Z),
					new Vector3(max.X - min.X, 0, 0),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(max.X, max.Y, max.Z), new Vector3(min.X - max.X, 0, 0),
					new Vector3(0, min.Y - max.Y, 0),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(max.X, max.Y, max.Z), new Vector3(0, min.Y - max.Y, 0),
					new Vector3(0, 0, min.Z - max.Z),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(max.X, max.Y, max.Z), new Vector3(0, 0, min.Z - max.Z),
					new Vector3(min.X - max.X, 0, 0),
					_squareBoundary),

				// edges
				new BoundEdgeCollision(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X - min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, min.Y, min.Z), new Vector3(0, max.Y - min.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, min.Y, min.Z), new Vector3(0, 0, max.Z - min.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, max.Y, max.Z), new Vector3(max.X - min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, max.Y, max.Z), new Vector3(0, min.Y - max.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, max.Y, max.Z), new Vector3(0, 0, min.Z - max.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(max.X, min.Y, min.Z), new Vector3(0, 0, max.Z - min.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(max.X, min.Y, min.Z), new Vector3(0, max.Y - min.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(max.X, max.Y, max.Z), new Vector3(0, min.Y - max.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(max.X, max.Y, max.Z), new Vector3(0, 0, min.Z - max.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, max.Y, min.Z), new Vector3(max.X - min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(min.X, min.Y, max.Z), new Vector3(max.X - min.X, 0, 0), _maxScale1)
			);
		}
	}
}