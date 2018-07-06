using System;
using Microsoft.Xna.Framework;

namespace Singularity.Events
{
	public class CollisionEventArgs : EventArgs
	{
		public CollisionEventArgs(Vector3 position, Vector3 normal, GameObject collider, GameObject collidable,
			GameScene scene)
		{
			Position = position;
			Normal = normal;
			Collider = collider;
			Collidable = collidable;
			Scene = scene;
		}

		public Vector3 Position { get; }
		public Vector3 Normal { get; }

		public GameObject Collider { get; }
		public GameObject Collidable { get; }

		public GameScene Scene { get; }
	}
}