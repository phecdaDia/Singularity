using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Collisions.Multi;
using Singularity.GameObjects.Interfaces;

namespace SingularityTest.GameObjects.ChildTest
{
	public class ParentBlock : GameObject, ICollidable
	{
		public ParentBlock()
		{
			this.SetModel("cubes/cube1");
			this.SetScale(5, 1, 5);
			this.SetCollision(new BoxCollision(new Vector3(-0.5f), new Vector3(0.5f)));


			this.OnCollisionEvent += this.ParentBlock_OnCollisionEvent;

		}

		private void ParentBlock_OnCollisionEvent(Object sender, Singularity.Events.CollisionEventArgs e)
		{
			if (!(e.Collider is ChildBall))
			{
				return;
			}


			// now try to get the ball as child with translation override
			this.AddChild(e.Collider, ChildProperties.Translation | ChildProperties.KeepPositon);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{

		}
	}
}
