using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Core;
using Singularity.Core.Collisions;
using Singularity.Core.Events;
using Singularity.Core.GameObjects.Interfaces;

namespace SingularityTest.GameObjects
{
	public class TestBallObject : GameObject, ICollidable, ICollider, IInertia
	{
		private static readonly float TEN_DEGREES_IN_RAD = 0.174533f;

		public TestBallObject()
		{
			SetModel("sphere");
			SetCollision(new SphereCollision(1.0f));

			AddCollisionEvent(HandleCollisionEvent);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			AddInertia(0, -20, 0, gameTime);

			//Console.WriteLine($"Inertia Energy: {(this.Inertia + new Vector3(this.Position.Y + 8)).Length()}");
		}

		public void HandleCollisionEvent(CollisionEventArgs e)
		{
			var dot = Vector3.Dot(e.Normal, Inertia);
			var nNor = 2 * dot * e.Normal;

			var nIner = nNor - Inertia;

			nIner *= Inertia.Length() / nIner.Length();


			SetInertia(-nIner);

			//Console.WriteLine($"NORMAL: {normal}, {normal.Length()}, ARCCOS: {Math.Acos(normal.Y)}");
			//this.SetInertia(0, 0, 0);
		}
	}
}