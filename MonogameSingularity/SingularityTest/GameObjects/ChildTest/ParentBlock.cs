using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Core;
using Singularity.Core.Collisions.Multi;
using Singularity.Core.Events;
using Singularity.Core.GameObjects;

namespace SingularityTest.GameObjects.ChildTest
{
	public class ParentBlock : GameObject, ICollidable
	{
		public ParentBlock()
		{
			SetModel("cubes/cube1");
			SetScale(5, 2f, 5);
			SetCollision(new BoxCollision(new Vector3(-0.5f), new Vector3(0.5f)));


			CollisionEvent += ParentBlockCollisionEvent;
		}

		private void ParentBlockCollisionEvent(object sender, CollisionEventArgs e)
		{
			//if (!(e.Collider is ChildBall)) return;


			// now try to get the ball as child with translation override
			AddChild(e.Collider, ChildProperties.Translation | ChildProperties.KeepPositon);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
		}
	}
}