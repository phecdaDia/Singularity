﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

			this.SetModel("cylinders/cylinder5");
			this.SetScale(1f, 1, 1f);


			//this.SetRotation(MathHelper.PiOver2, 0, 0);
			this.AddScript((scene, go, time) => AddRotation(0.4f, 0.6f, 1f, time));
		}
	}
}
