using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Events;
using Singularity.GameObjects;

namespace SingularityTest.GameObjects
{
	internal class MovingBlock : CollidableModelObject
	{
		private readonly float Speed;
		private readonly Queue<Vector3> Targets;
		private int CollisionFrames;
		private float Timer;

		public MovingBlock(string modelPath, float speed, params Vector3[] targets) : base(modelPath)
		{
			this.SetPosition(targets[0]);

			DrawChildren = false;
			UpdateChildren = false;

			this.Targets = new Queue<Vector3>();
			foreach (var vector in targets) this.Targets.Enqueue(vector);
			this.Speed = speed;
			this.Timer = float.PositiveInfinity;
			this.AddCollisionEvent(this.CollEvent);
		}

		private void CollEvent(CollisionEventArgs e)
		{
			//Console.WriteLine($"Collided after {this.CollisionFrames} frames");
			this.CollisionFrames = 0;


			Console.WriteLine($"{e.Position} | {e.Position - e.Collidable.Position}");
			//get angle of the slope
			var angleOfSlope =
				Math.Acos(e.Normal.Y); // we can do this, as this is a very special case. |n| is always 1 and the angle we're comparing against is (0, 1, 0), UnitY

			if (-0.18 <= angleOfSlope && angleOfSlope <= 0.18) // about 10 degrees
			{
				//e.Scene.RemoveObject(e.Collider);
				//e.Collider.SetPosition(e.Collider.Position - Position);
				var player = e.Collider;
				if (!this.ChildObjects.Contains(player))
					this.AddChild(e.Collider, ChildProperties.Translation | ChildProperties.KeepPositon);
			}
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			var childCopy = this.ChildObjects.ToArray();


			// if we didn't collide with it for 2 frames, we're no longer it's child.

			var movement = this.Targets.First() - this.Position;
			var nMovement = movement;
			nMovement.Normalize();
			nMovement *= this.Speed;
			var m = (int)-Math.Max(1, nMovement.Y * 5) + 10;
			//Console.WriteLine($"{m}");

			if (this.CollisionFrames > m)
			{
				if (this.ChildObjects.Count > 0)
					//Console.WriteLine($"Removing children");

					foreach (var go in childCopy)
						go.RemoveParent();
			}

			// count collision frames
			this.CollisionFrames++;


			if (Vector3.Distance(this.Position, this.Targets.First()) < 0.1f)
			{
				this.Timer = 0.7f;
				this.Targets.Enqueue(this.Targets.Dequeue());
			}

			if (this.Timer <= 0.0f)
			{
				//this.Timer = float.PositiveInfinity;
				if (movement.Length() < this.Speed * gameTime.ElapsedGameTime.TotalSeconds)
				{
					this.SetPosition(this.Targets.First());
				}
				else
				{
					this.AddPosition(nMovement, gameTime);
				}
			}
			else
			{
				this.Timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

			base.Update(scene, gameTime);
		}
	}
}
