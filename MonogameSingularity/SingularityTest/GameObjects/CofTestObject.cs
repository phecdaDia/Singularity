using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Core;
using Singularity.Core.GameObjects;

namespace SingularityTest.GameObjects
{
	public class CofTestObject : GameObject, ICollidable
	{
		public bool DidSpawn;
		public double ElapsedTime;

		public CofTestObject()
		{
			//this.SetPosition(3, 0, 0);
			SetModel("sphere");
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			AddRotation(0, 1, 0, gameTime);

			if (DidSpawn) return;

			ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

			if (ElapsedTime >= 2.0d)
			{
				// Spawn a new child.
				DidSpawn = true;

				var cof = new CofTestObject().SetPosition(5, 1, 0);
				SceneManager.GetCurrentScene().SpawnObject(cof, this);
			}
		}
	}
}