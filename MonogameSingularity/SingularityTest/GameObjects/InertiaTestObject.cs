using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity;
using Singularity.Code.GameObjects.Interfaces;

namespace SingularityTest.GameObjects
{
	class InertiaTestObject : GameObject, IInertia
	{
		private Vector3 Inertia = new Vector3();

		private Vector3 Gravity = new Vector3(0, -1, 0);

		public InertiaTestObject()
		{
			this.SetModel("sphere");
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (this.GetHierarchyPosition().Y <= 0)
			{
				this.Inertia.Y = -this.Inertia.Y * 0.99f;

				if (this.Inertia.Y <= 0.01f) this.Inertia.Y = 1.0f;
			}

			// add gravity
			this.Inertia += this.Gravity * (float) gameTime.ElapsedGameTime.TotalSeconds;

			this.AddPosition(this.Inertia);
		}

		public void SetInertia(Vector3 inertia)
		{
			this.Inertia = inertia;
		}

		public Vector3 GetInertia()
		{
			return this.Inertia;
		}
	}
}
