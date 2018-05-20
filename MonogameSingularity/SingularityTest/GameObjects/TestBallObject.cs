using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Collisions;
using Singularity.GameObjects.Interfaces;

namespace SingularityTest.GameObjects
{
	public class TestBallObject : GameObject, ICollidable, ICollider, IInertia
	{
		private static readonly float TEN_DEGREES_IN_RAD = 0.174533f;

		public TestBallObject() : base()
		{
			this.SetModel("sphere");
			this.SetCollision(new SphereCollision(1.0f));

			this.AddCollisionEvent(HandleCollisionEvent);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			this.AddInertia(new Vector3(0, -20, 0) * (float) gameTime.ElapsedGameTime.TotalSeconds);

			//Console.WriteLine($"Inertia Energy: {(this.Inertia + new Vector3(this.Position.Y + 8)).Length()}");
		}

		public void HandleCollisionEvent(GameObject collidable, GameScene scene, Vector3 position, Vector3 normal)
		{

			var dot = Vector3.Dot(normal, this.Inertia);
			var nNor = 2 * dot * normal;

			var nIner = nNor - this.Inertia;

			nIner *= this.Inertia.Length() / nIner.Length();


			this.SetInertia(-nIner);

			//Console.WriteLine($"NORMAL: {normal}, {normal.Length()}, ARCCOS: {Math.Acos(normal.Y)}");
			//this.SetInertia(0, 0, 0);
		}
	}
}
