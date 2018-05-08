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

			this.SetModel("cubes/cube1");
			this.SetCollision(new BoxCollision(this, new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
			this.AddScript((scene, go, time) => AddRotation(new Vector3(0.4f, 0.6f, 1f) * (float) time.ElapsedGameTime.TotalSeconds));
		}
	}
}
