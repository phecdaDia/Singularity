using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public class RayCollisionPoint
	{
		public RayCollisionPoint(GameObject collidable, Vector3 position, Vector3 normal, float rayDistance)
		{
			this.Collidable = collidable;
			this.Position = position;
			this.Normal = normal;
			this.DidCollide = true;
			this.RayDistance = rayDistance;
		}

		public RayCollisionPoint()
		{
			this.RayDistance = float.PositiveInfinity;
			this.DidCollide = false;
		}

		public Vector3 Position { get; }
		public Vector3 Normal { get; }
		public bool DidCollide { get; private set; }
		public float RayDistance { get; }
		public GameObject Collidable { get; }


		public void SetCollide(bool collide)
		{
			this.DidCollide = collide;
		}
	}
}