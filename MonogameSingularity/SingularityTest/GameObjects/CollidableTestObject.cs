using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Collisions;
using Singularity.GameObjects;
using Singularity.GameObjects.Interfaces;
using Singularity.Utilities;

namespace SingularityTest.GameObjects
{
	public class CollidableTestObject : EmptyGameObject, ICollidable, IGlobal
	{
		public CollidableTestObject()
		{
			bool _squareBoundary(float f1, float f2) => 0 <= f1 && f1 <= 1 && 0 <= f2 && f2 <= 1;

			this.SetModel("cubes/cube1");
			this.SetCollision(new MultiCollision(this,
				new BoundPlaneCollision(this, new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector3(0, 1, 0),
					_squareBoundary),
				new BoundPlaneCollision(this, new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 1, 0), new Vector3(0, 0, 1),
					_squareBoundary),
				new BoundPlaneCollision(this, new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, 1), new Vector3(1, 0, 0),
					_squareBoundary)
			));
			//this.AddRotation(0, MathHelper.PiOver2, 0);
		}
	}
}
