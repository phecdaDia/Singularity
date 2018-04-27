using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Code.Events
{
	public class GameObjectCollisionEvent : EventArgs
	{
		public GameObject GameObject { get; private set; }

		public GameScene GameScene { get; private set; }

		public Vector3 Movement { get; private set; }

		public GameObjectCollisionEvent(GameObject gameObject, GameScene gameScene, Vector3 movement)
		{
			GameObject = gameObject;
			GameScene = gameScene;
			Movement = movement;
		}
	}
}
