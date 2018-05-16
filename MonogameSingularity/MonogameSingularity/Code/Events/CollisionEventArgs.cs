using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Events
{
	public class CollisionEventArgs : EventArgs
	{
		public Vector3 Position { get; private set; }
		public Vector3 Normal { get; private set; }
		
		public GameObject Collidable { get; private set; }

		public GameScene Scene { get; private set; }

		public CollisionEventArgs(Vector3 position, Vector3 normal, GameObject collidable, GameScene scene)
		{
			Position = position;
			Normal = normal;
			Collidable = collidable;
			Scene = scene;
		}
	}
}
