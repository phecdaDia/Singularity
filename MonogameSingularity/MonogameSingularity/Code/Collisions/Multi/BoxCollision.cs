using Microsoft.Xna.Framework;

namespace Singularity.Collisions.Multi
{
	public class BoxCollision : MultiCollision
	{
		public BoxCollision(Vector3 Min, Vector3 Max)
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
				new BoundPlaneCollision(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(Max.X - Min.X, 0, 0),
					new Vector3(0, Max.Y - Min.Y, 0),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, Max.Y - Min.Y, 0),
					new Vector3(0, 0, Max.Z - Min.Z),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, 0, Max.Z - Min.Z),
					new Vector3(Max.X - Min.X, 0, 0),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(Max.X, Max.Y, Max.Z), new Vector3(Min.X - Max.X, 0, 0),
					new Vector3(0, Min.Y - Max.Y, 0),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, Min.Y - Max.Y, 0),
					new Vector3(0, 0, Min.Z - Max.Z),
					_squareBoundary),
				new BoundPlaneCollision(new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, 0, Min.Z - Max.Z),
					new Vector3(Min.X - Max.X, 0, 0),
					_squareBoundary),

				// edges
				new BoundEdgeCollision(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, Max.Y - Min.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, 0, Max.Z - Min.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Max.Y, Max.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Max.Y, Max.Z), new Vector3(0, Min.Y - Max.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Max.Y, Max.Z), new Vector3(0, 0, Min.Z - Max.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(Max.X, Min.Y, Min.Z), new Vector3(0, 0, Max.Z - Min.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(Max.X, Min.Y, Min.Z), new Vector3(0, Max.Y - Min.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, Min.Y - Max.Y, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, 0, Min.Z - Max.Z), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Max.Y, Min.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(new Vector3(Min.X, Min.Y, Max.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1)
			);
		}
	}
}