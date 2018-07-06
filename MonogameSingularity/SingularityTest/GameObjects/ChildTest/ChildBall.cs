using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Collisions;
using Singularity.Events;
using Singularity.GameObjects.Interfaces;

namespace SingularityTest.GameObjects.ChildTest
{
	public class ChildBall : GameObject, IInertia, ICollider
	{
		private readonly Vector3 Gravity = new Vector3(0, -10, 0);

		public ChildBall()
		{
			SetModel("sphere");
			SetCollision(new SphereCollision());

			CollisionEvent += ChildBallCollisionEvent;
		}

		private void ChildBallCollisionEvent(object sender, CollisionEventArgs e)
		{
			var dot = Vector3.Dot(e.Normal, Inertia);
			var nNor = 2 * dot * e.Normal;

			var nIner = nNor - Inertia;

			nIner *= Inertia.Length() / nIner.Length();


			SetInertia(-nIner);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			AddInertia(Gravity, gameTime);

			if (Inertia.Y < 0) RemoveParent();
			//Console.WriteLine($"My parent is: {this.ParentObject}");
		}
	}
}