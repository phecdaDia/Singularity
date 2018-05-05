﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Code;
using Singularity.Code.GameObjects;

namespace ExampleMarble.GameObjects
{
	public class MarbleObject : GameObject
	{
		private float verticalRotation, horizontalRotation;
		private float distance = 5.0f;

		private Vector3 Inertia;

		public MarbleObject()
		{
			this.SetModel(ModelManager.GetModel("sphere"));

			this.Inertia = new Vector3(0, 0, 0);
		}
		public override void Update(GameScene scene, GameTime gameTime)
		{
			CaptureMouse(scene.Game);

			if (horizontalRotation >= MathHelper.TwoPi) horizontalRotation -= MathHelper.TwoPi;
			else if (horizontalRotation < 0f) horizontalRotation += MathHelper.TwoPi;

			if (verticalRotation > MathHelper.PiOver2)
				verticalRotation = MathHelper.PiOver2;
			else if (verticalRotation < -MathHelper.PiOver2)
				verticalRotation = -MathHelper.PiOver2;

			// calculate forward vector
			Vector3 target = new Vector3((float)Math.Cos(horizontalRotation), (float)Math.Sin(horizontalRotation), (float)Math.Sin(verticalRotation));

			var movement = new Vector3();
			var ks = Keyboard.GetState();

			if (KeyboardManager.IsKeyPressed(Keys.R))
			{
				this.Inertia = new Vector3();
				this.SetPosition(new Vector3(0, 0, 10));
			}


			target.Normalize();

			// Calculate orthagonal vectors
			Vector3 forward = target;
			forward.Z = 0;
			Vector3 backwards = -forward;

			Vector3 right = new Vector3(forward.Y, -forward.X, 0);
			Vector3 left = -right;

			// normalize all vectors
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

			// add gravity
			movement += new Vector3(0, 0, -1);

			movement *= (float) gameTime.ElapsedGameTime.TotalSeconds;

			// add movement to inertia
			this.Inertia += movement;

			TryMove(scene);

			// Inertia decay
			this.Inertia = new Vector3(0.9f * this.Inertia.X, 0.9f * this.Inertia.Y, this.Inertia.Z);

			scene.SetCameraPosition(this.GetHierarchyPosition() + this.distance * (backwards + new Vector3(0, 0, 1f)));
			scene.SetAbsoluteCameraTarget(this.GetHierarchyPosition());
		}

		public void TryMove(GameScene scene)
		{
			

		}

		private void CaptureMouse(SingularityGame game)
		{
			int pos = 400;

			if (!game.IsActive) return;

			MouseState ms = Mouse.GetState();

			this.horizontalRotation += (ms.X - pos) / 100.0f;
			this.verticalRotation += (ms.Y - pos) / 100.0f;

			this.distance = 5.0f + ms.ScrollWheelValue / 1200f;

			Mouse.SetPosition(pos, pos);
			
		}
	}
}