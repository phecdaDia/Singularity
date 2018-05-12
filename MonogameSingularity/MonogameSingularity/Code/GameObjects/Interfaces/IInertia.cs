using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.GameObjects.Interfaces
{
	public interface IInertia
	{
		void SetInertia(Vector3 inertia);
		Vector3 GetInertia();
	}
}
