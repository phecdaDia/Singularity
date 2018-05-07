using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Code;
using Singularity.Code.Collisions;
using Singularity.Code.GameObjects;
using Singularity.Code.GameObjects.Interfaces;
using Singularity.Code.Utilities;

namespace SingularityTest.GameObjects
{
	public class CollidableTestObject : EmptyGameObject, ICollidable, IGlobal
	{
		public CollidableTestObject()
		{
			this.SetModel("cubes/cube1");
			this.SetCollision(new BoundPlaneCollision(this, new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 1, 0), new Vector3(0, 0, 1), 
			(f1, f2) =>
			{
				Console.WriteLine($"{f1}, {f2}");
				
				return 0 <= f1 && f1 < 1 && 0 <= f2 && f2 < 1;
			}
			));
			//this.AddRotation(0, MathHelper.PiOver2, 0);
		}
	}
}
