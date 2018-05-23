using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity;
using Singularity.GameObjects.Interfaces;

namespace SingularityTest.GameObjects
{
	public class CofTestObject : GameObject, ICollidable
	{
		public double ElapsedTime = 0.0d;
		public Boolean DidSpawn = false;

		public CofTestObject() : base()
		{
			//this.SetPosition(3, 0, 0);
			this.SetModel("sphere");
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			this.AddRotation(0, (float) gameTime.ElapsedGameTime.TotalSeconds, 0);

			if (DidSpawn) return;

			this.ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

			if (ElapsedTime >= 2.0d)
			{
				// Spawn a new child.
				this.DidSpawn = true;

				var cof = new CofTestObject().SetPosition(5, 1, 0);
				SceneManager.GetCurrentScene().SpawnObject(cof, this);
			}
		}
	}
}
