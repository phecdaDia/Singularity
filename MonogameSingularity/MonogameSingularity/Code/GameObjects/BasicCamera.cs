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
		private double horizontalRotation;	// rotation around the z axis
		private double verticalRotation;	// rotation up and down
		public Boolean Is3DEnabled { get; private set; }

		public BasicCamera() : base()
		{
			this.horizontalRotation = 0.0d;
			this.verticalRotation = 0.0d;

			Mouse.SetPosition(200, 200); // capture the mouse
		}


		/// <summary>
		/// Enables <see cref="verticalRotation"/>
		/// </summary>
		/// <param name="enable"></param>
		public void Set3DEnabled(Boolean enable)
		{
			this.Is3DEnabled = enable;
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (!scene.Game.IsActive) return;


			// Capture Mouse
			MouseState mouseState = Mouse.GetState();
			var dx = mouseState.X - 200;
			var dy = 200 - mouseState.Y;

			this.horizontalRotation += dx / 100f;

			if (this.Is3DEnabled)
				this.verticalRotation += dy / 100f;


			Mouse.SetPosition(200, 200);

			// Constraint rotation

			if (horizontalRotation >= MathHelper.TwoPi) horizontalRotation -= MathHelper.TwoPi;
			else if (horizontalRotation < 0f) horizontalRotation += MathHelper.TwoPi;

			if (verticalRotation > MathHelper.PiOver2)
				verticalRotation = MathHelper.PiOver2;
			else if (verticalRotation < -MathHelper.PiOver2)
				verticalRotation = -MathHelper.PiOver2;

			// calculate forward vector
			Vector3 target = new Vector3((float)Math.Cos(horizontalRotation), (float)Math.Sin(horizontalRotation), Is3DEnabled ? (float)Math.Sin(verticalRotation) : 0f);

			var movement = new Vector3();
			var ks = Keyboard.GetState();

			target.Normalize();

			// Calculate orthagonal vectors
			Vector3 forward = target;
			forward.Z = 0;
			Vector3 backwards = -forward;

			Vector3 right = new Vector3(forward.Y, -forward.X, 0);
			Vector3 left = -right;

			// Buffer movement
			if (ks.IsKeyDown(Keys.W)) movement += forward;
			if (ks.IsKeyDown(Keys.S)) movement += backwards;

			if (ks.IsKeyDown(Keys.A)) movement += right;
			if (ks.IsKeyDown(Keys.D)) movement += left;

			if (movement.LengthSquared() > 0f) movement.Normalize();

			// test collision
			if (!scene.DoesCollide(this.Position + movement * (float)gameTime.ElapsedGameTime.TotalSeconds * 5f, 0.125f))
				this.AddPosition(movement * (float)gameTime.ElapsedGameTime.TotalSeconds * 5f);

			// update relative camera
			scene.SetCamera(this.Position, target);

			//Console.WriteLine($"{this.Position.X} - {this.Position.Y} - {this.Position.Z}");
		}
	}
}
