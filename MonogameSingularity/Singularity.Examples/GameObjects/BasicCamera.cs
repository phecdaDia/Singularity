using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Core.Collisions;

namespace Singularity.Core.GameObjects
{
	public class BasicCamera : GameObject, ICollider, ICameraController
	{
		private int DefaultX;
		private int DefaultY;

		private bool firstFrame = true;
		private double HorizontalRotation; // rotation around the z axis
		private double VerticalRotation; // rotation up and down

		public BasicCamera()
		{
			this.HorizontalRotation = 0.0d;
			this.VerticalRotation = 0.0d;


			this.DefaultX = 200;
			this.DefaultY = 200;
			Mouse.SetPosition(this.DefaultX, this.DefaultY); // capture the mouse

			this.SetCollision(new SphereCollision(0.25f));
		}

		public bool Is3DEnabled { get; private set; }

		public void SetCamera(GameScene scene)
		{
			var target = this.GetCameraTarget();

			scene.SetCamera(this.GetHierarchyPosition(), target);
		}

		/// <summary>
		///     Enables <see cref="VerticalRotation" />
		/// </summary>
		/// <param name="enable"></param>
		public BasicCamera Set3DEnabled(bool enable)
		{
			this.Is3DEnabled = enable;

			return this;
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (!scene.Game.IsActive) return;


			if (this.firstFrame)
			{
				this.firstFrame = false;

				return;
			}

			var mouseState = Mouse.GetState();
			// Capture Mouse
			var dx = mouseState.X - this.DefaultX;
			var dy = this.DefaultY - mouseState.Y;

			this.HorizontalRotation += dx / 100f;

			if (this.Is3DEnabled)
				this.VerticalRotation += dy / 100f;

			this.DefaultX = mouseState.X;
			this.DefaultY = mouseState.Y;

			if (mouseState.X <= 100 || mouseState.X >= 300)
			{
				Mouse.SetPosition(200, mouseState.Y);
				this.DefaultX = 200;
			}

			mouseState = Mouse.GetState();

			if (mouseState.Y <= 100 || mouseState.Y >= 300)
			{
				Mouse.SetPosition(mouseState.X, 200);
				this.DefaultY = 200;
			}

			//MouseState ms = Mouse.GetState();
			//DefaultX = ms.X;
			//DefaultY = ms.Y;

			// Constraint rotation

			if (this.HorizontalRotation >= MathHelper.Pi) this.HorizontalRotation -= MathHelper.TwoPi;
			else if (this.HorizontalRotation < -MathHelper.Pi)
				this.HorizontalRotation += MathHelper.TwoPi;

			if (this.VerticalRotation > MathHelper.PiOver2)
				this.VerticalRotation = MathHelper.PiOver2;
			else if (this.VerticalRotation < -MathHelper.PiOver2)
				this.VerticalRotation = -MathHelper.PiOver2;

			// calculate forward vector
			var target = this.GetCameraTarget();

			var movement = new Vector3();
			var ks = Keyboard.GetState();

			target.Normalize();

			// Calculate orthagonal vectors
			var forward = target;
			forward.Y = 0;
			var backwards = -forward;

			var right = new Vector3(forward.Z, 0, -forward.X);
			var left = -right;

			// normalize vectors

			forward.Normalize();
			backwards.Normalize();

			right.Normalize();
			left.Normalize();

			// Buffer movement
			if (ks.IsKeyDown(Keys.W)) movement += forward;
			if (ks.IsKeyDown(Keys.S)) movement += backwards;

			if (ks.IsKeyDown(Keys.A)) movement += right;
			if (ks.IsKeyDown(Keys.D)) movement += left;

			if (movement.LengthSquared() > 0f) movement.Normalize();

			this.AddPosition(movement * (float) gameTime.ElapsedGameTime.TotalSeconds * 5f);
		}

		public BasicCamera SetCameraTarget(Vector3 target)
		{
			//Console.WriteLine($"Cpos1: {target}");

			target.Normalize();

			//Console.WriteLine($"Campos: {target}");

			var horizontal = (float) Math.Acos(target.X);
			var vertical = (float) Math.Asin(target.Y);

			//var h2 = (float) Math.Asin(target.Z);
			//Console.WriteLine($"{horizontal} == {h2}?, {vertical}");

			return this.SetCameraTarget(horizontal, vertical);
		}

		public BasicCamera SetCameraTarget(float horizontal, float vertical)
		{
			this.HorizontalRotation = horizontal;
			this.VerticalRotation = vertical;
			return this;
		}

		public Vector3 GetCameraTarget()
		{
			return new Vector3((float) Math.Cos(this.HorizontalRotation), this.Is3DEnabled ? (float) Math.Sin(this.VerticalRotation) : 0f,
				(float) Math.Sin(this.HorizontalRotation));
		}
	}
}