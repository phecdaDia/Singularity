using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Core;
using Singularity.Core.Collisions;
using Singularity.Core.GameObjects;

namespace Singularity.Examples.GameObjects
{
	public class StaticCamera : GameObject, ICollider, ICameraController
	{
		private int DefaultX;
		private int DefaultY;

		private bool firstFrame = true;
		private double HorizontalRotation; // rotation around the z axis
		private double VerticalRotation; // rotation up and down

		public StaticCamera()
		{
			HorizontalRotation = 0.0d;
			VerticalRotation = 0.0d;


			DefaultX = 200;
			DefaultY = 200;
			Mouse.SetPosition(DefaultX, DefaultY); // capture the mouse

			SetCollision(new SphereCollision(0.25f));
		}

		public void SetCamera(GameScene scene)
		{
			var target = GetCameraTarget();

			scene.SetCamera(this.GetHierarchyPosition(), target);
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (!scene.Game.IsActive) return;


			if (firstFrame)
			{
				firstFrame = false;

				return;
			}
		}

		public StaticCamera SetCameraTarget(Vector3 target)
		{
			//Console.WriteLine($"Cpos1: {target}");

			target.Normalize();

			//Console.WriteLine($"Campos: {target}");

			var horizontal = (float)Math.Acos(target.X);
			var vertical = (float)Math.Asin(target.Y);

			//var h2 = (float) Math.Asin(target.Z);
			//Console.WriteLine($"{horizontal} == {h2}?, {vertical}");

			return SetCameraTarget(horizontal, vertical);
		}

		public StaticCamera SetCameraTarget(float horizontal, float vertical)
		{
			HorizontalRotation = horizontal;
			VerticalRotation = vertical;
			return this;
		}

		public Vector3 GetCameraTarget()
		{
			return new Vector3((float)Math.Cos(HorizontalRotation), (float)Math.Sin(VerticalRotation),
				(float)Math.Sin(HorizontalRotation));
		}
	}
}