using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.GameObjects.Interfaces;

namespace Singularity.GameObjects
{
	public class FullCollidableModelObject : ModelObject, ICollidable, ICollider
	{
		public FullCollidableModelObject(string modelPath) : base(modelPath)
		{
		}
	}
}
