using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Code.Collisions;
using Singularity.Collisions;
using Singularity.Collisions.Multi;
using Singularity.GameObjects;
using Singularity.GameObjects.Interfaces;
using Singularity.Utilities;

namespace SingularityTest.GameObjects
{
	public class CollidableTestObject : EmptyGameObject, ICollidable, IGlobal
	{
		public CollidableTestObject()
		{

			this.SetModel("cylinders/cylinder3");
			this.SetScale(1f, 1, 1f);
			this.SetCollision(
				new MultiCollision(this,
					new BoundEdgeCollision(this, new Vector3(0, -0.5f, 0), new Vector3(0, 1, 0), 1.0f, (scale, distance) => 0 <= scale && scale <= 1),
					new BoundPlaneCollision(this, new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), (f1, f2) => Math.Abs(f1) + Math.Abs(f2) <= 1),
					new BoundPlaneCollision(this, new Vector3(0, -0.5f, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), (f1, f2) => Math.Abs(f1) + Math.Abs(f2) <= 1)

				)
			);
			//this.AddScript((scene, go, time) => AddRotation(new Vector3(0.4f, 0.6f, 1f) * (float) time.ElapsedGameTime.TotalSeconds));
		}
	}
}
