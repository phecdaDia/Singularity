using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Code.GameObjects
{
	public class BasicCamera : GameObject
	{
		private double horizontalRotation;
		private double verticalRotation;

		public BasicCamera() : base()
		{
			this.horizontalRotation = 0.0d;
			this.verticalRotation = 0.0d;

			Mouse.SetPosition(200, 200);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			MouseState mouseState = Mouse.GetState();
			var dx = 200 - mouseState.X;
			var dy = 200 - mouseState.Y;

			this.horizontalRotation += dx / 1000f;
			this.verticalRotation += dy / 1000f;


			Mouse.SetPosition(200, 200);


			Vector3 target = new Vector3((float)Math.Sin(horizontalRotation), 0/*(float)Math.Sin(verticalRotation)*/, (float)Math.Cos(horizontalRotation));

			var movement = new Vector3();
			var ks = Keyboard.GetState();

			target.Normalize();

			Vector3 forward = target;
			Vector3 backwards = -forward;

			Vector3 right = new Vector3(forward.Z, 0, -forward.X);
			Vector3 left = -right;


			if (ks.IsKeyDown(Keys.W)) movement += forward;
			if (ks.IsKeyDown(Keys.S)) movement += backwards;

			if (ks.IsKeyDown(Keys.A)) movement += left;
			if (ks.IsKeyDown(Keys.D)) movement += right;

			if (movement.LengthSquared() > 0f) movement.Normalize();
			this.Position += movement * (float)gameTime.ElapsedGameTime.TotalSeconds;


			scene.SetCamera(this.Position, target);

			Console.WriteLine($"{this.Position.X} - {this.Position.Y} - {this.Position.Z}");
		}
	}
}
