using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity;
using Singularity.GameObjects.Interfaces;

namespace SingularityTest.GameObjects
{
	class InertiaTestObject : GameObject, IInertia
	{

		private readonly Vector3 Gravity = new Vector3(0, -1, 0);

		public InertiaTestObject()
		{
			this.SetModel("sphere");
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (this.GetHierarchyPosition().Y <= 0)
			{
				this.SetInertia(this.Inertia.X, -this.Inertia.Y, this.Inertia.Z);

				if (this.Inertia.Y <= 0.01f) this.SetInertia(this.Inertia.X, 0.5f, this.Inertia.Z);
			}

			// add gravity
			this.AddInertia(this.Gravity * (float) gameTime.ElapsedGameTime.TotalSeconds);

			this.AddPosition(this.Inertia);
		}
	}
}
