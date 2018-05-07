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
			this.SetCollision(new PlaneCollision(this, new Vector3(), new Vector3(0, 1, 0), new Vector3(0, 0, 1)));
			this.AddRotation(0, MathHelper.PiOver2, 0);
		}
	}
}
