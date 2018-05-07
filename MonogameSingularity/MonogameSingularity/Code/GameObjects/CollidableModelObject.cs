using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code.GameObjects.Interfaces;

namespace Singularity.Code.GameObjects
{
	public class CollidableModelObject : ModelObject, ICollidable
	{
		public CollidableModelObject(string modelPath) : this(ModelManager.GetModel(modelPath))
		{}

		public CollidableModelObject(Model model) : base(model)
		{
			
		}
	}
}
