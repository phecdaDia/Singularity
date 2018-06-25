using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Collisions;
using Singularity.GameObjects.Interfaces;

namespace Singularity.GameObjects
{
	public class BasicCamera : GameObject, ICollider, ICameraController
	{
		private double HorizontalRotation;	// rotation around the z axis
		private double VerticalRotation;	// rotation up and down
		public Boolean Is3DEnabled { get; private set; }
		
		private int DefaultX;
		private int DefaultY;

		public BasicCamera() : base()
		{
			this.HorizontalRotation = 0.0d;
			this.VerticalRotation = 0.0d;

			
			DefaultX = 200;
			DefaultY = 200;
			Mouse.SetPosition(DefaultX, DefaultY); // capture the mouse

			this.SetCollision(new SphereCollision(0.25f));
		}

		/// <summary>
		/// Enables <see cref="VerticalRotation"/>
		/// </summary>
		/// <param name="enable"></param>
		public BasicCamera Set3DEnabled(Boolean enable)
		{
			this.Is3DEnabled = enable;

			return this;
		}

		private Boolean firstFrame = true;

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (!scene.Game.IsActive) return;


			if (this.firstFrame)
			{
				this.firstFrame = false;

				return;
			}

			MouseState mouseState = Mouse.GetState();
			// Capture Mouse
			var dx = mouseState.X - DefaultX;
			var dy = DefaultY - mouseState.Y;

			this.HorizontalRotation += dx / 100f;

			if (this.Is3DEnabled)
				this.VerticalRotation += dy / 100f;

			DefaultX = mouseState.X;
			DefaultY = mouseState.Y;

			if (mouseState.X <= 100 || mouseState.X >= 300)
			{
				Mouse.SetPosition(200, mouseState.Y);
				DefaultX = 200;
			}

			mouseState = Mouse.GetState();

			if (mouseState.Y <= 100 || mouseState.Y >= 300)
			{
				Mouse.SetPosition(mouseState.X, 200);
				DefaultY = 200;

			}

			//MouseState ms = Mouse.GetState();
			//DefaultX = ms.X;
			//DefaultY = ms.Y;

			// Constraint rotation

			if (HorizontalRotation >= MathHelper.Pi) HorizontalRotation -= MathHelper.TwoPi;
			else if (HorizontalRotation < -MathHelper.Pi) HorizontalRotation += MathHelper.TwoPi;

			if (VerticalRotation > MathHelper.PiOver2)
				VerticalRotation = MathHelper.PiOver2;
			else if (VerticalRotation < -MathHelper.PiOver2)
				VerticalRotation = -MathHelper.PiOver2;

			// calculate forward vector
			Vector3 target = GetCameraTarget();

			var movement = new Vector3();
			var ks = Keyboard.GetState();

			target.Normalize();

			// Calculate orthagonal vectors
			Vector3 forward = target;
			forward.Y = 0;
			Vector3 backwards = -forward;

			Vector3 right = new Vector3(forward.Z, 0, -forward.X);
			Vector3 left = -right;

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
			
			this.AddPosition(movement * (float)gameTime.ElapsedGameTime.TotalSeconds * 5f);
		}

		public void SetCamera(GameScene scene)
		{
			Vector3 target = GetCameraTarget();

			scene.SetCamera(this.Position, target);
		}

		public BasicCamera SetCameraTarget(Vector3 target)
		{
			//Console.WriteLine($"Cpos1: {target}");

			target.Normalize();

			//Console.WriteLine($"Campos: {target}");

			var horizontal = (float)Math.Acos(target.X);
			var vertical = (float)Math.Asin(target.Y);

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
			return new Vector3((float) Math.Cos(HorizontalRotation), Is3DEnabled? (float) Math.Sin(VerticalRotation) : 0f, (float) Math.Sin(HorizontalRotation));
		}
	}
}
