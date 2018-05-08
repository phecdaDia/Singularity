using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Code.Collisions;

namespace Singularity.Collisions.Multi
{
	public class BoxCollision : MultiCollision
	{
		public BoxCollision(GameObject parent, Vector3 Min, Vector3 Max) : base(parent)
		{
			bool _squareBoundary(float f1, float f2) => 0 <= f1 && f1 <= 1 && 0 <= f2 && f2 <= 1;
			bool _maxScale1(float scale, float distance) => 0 <= scale && scale <= 1;
			
			this.AddCollisions(
				new BoundPlaneCollision(this.Parent, new Vector3(Min.X, Min.Y, Min.Z), new Vector3(Max.X - Min.X, 0, 0), new Vector3(0, Max.Y - Min.Y, 0),
					_squareBoundary),
				new BoundPlaneCollision(this.Parent, new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, Max.Y - Min.Y, 0), new Vector3(0, 0, Max.Z - Min.Z),
					_squareBoundary),
				new BoundPlaneCollision(this.Parent, new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, 0, Max.Z - Min.Z), new Vector3(Max.X - Min.X, 0, 0),
					_squareBoundary),
				new BoundPlaneCollision(this.Parent, new Vector3(Max.X, Max.Y, Max.Z), new Vector3(Min.X - Max.X, 0, 0), new Vector3(0, Min.Y - Max.Y, 0),
					_squareBoundary),
				new BoundPlaneCollision(this.Parent, new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, Min.Y - Max.Y, 0), new Vector3(0, 0, Min.Z - Max.Z),
					_squareBoundary),
				new BoundPlaneCollision(this.Parent, new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, 0, Min.Z - Max.Z), new Vector3(Min.X - Max.X, 0, 0),
					_squareBoundary),


				// edges
				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Min.Y, Min.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, Max.Y - Min.Y, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Min.Y, Min.Z), new Vector3(0, 0, Max.Z - Min.Z), _maxScale1),

				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Max.Y, Max.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Max.Y, Max.Z), new Vector3(0, Min.Y - Max.Y, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Max.Y, Max.Z), new Vector3(0, 0, Min.Z - Max.Z), _maxScale1),

				new BoundEdgeCollision(this.Parent, new Vector3(Max.X, Min.Y, Min.Z), new Vector3(0, 0, Max.Z - Min.Z), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Max.X, Min.Y, Min.Z), new Vector3(0, Max.Y - Min.Y, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, Min.Y - Max.Y, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Max.X, Max.Y, Max.Z), new Vector3(0, 0, Min.Z - Max.Z), _maxScale1),

				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Max.Y, Min.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1),
				new BoundEdgeCollision(this.Parent, new Vector3(Min.X, Min.Y, Max.Z), new Vector3(Max.X - Min.X, 0, 0), _maxScale1)
			);
		}
	}
}
