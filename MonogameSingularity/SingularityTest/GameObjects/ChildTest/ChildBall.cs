using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
			this.SetModel("sphere");
			this.SetCollision(new SphereCollision());

			this.OnCollisionEvent += this.ChildBall_OnCollisionEvent;

		}

		private void ChildBall_OnCollisionEvent(Object sender, CollisionEventArgs e)
		{
			var dot = Vector3.Dot(e.Normal, this.Inertia);
			var nNor = 2 * dot * e.Normal;

			var nIner = nNor - this.Inertia;

			nIner *= this.Inertia.Length() / nIner.Length();


			this.SetInertia(-nIner);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			this.AddInertia(Gravity * (float) gameTime.ElapsedGameTime.TotalSeconds);

			if (this.Inertia.Y < 0)
			{
				this.RemoveParent();
			}
			//Console.WriteLine($"My parent is: {this.ParentObject}");
		}
	}
}
