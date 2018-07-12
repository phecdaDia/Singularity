using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class RayCollisionPoint
	{
		public RayCollisionPoint(GameObject collidable, Vector3 position, Vector3 normal, float rayDistance)
		{
			Collidable = collidable;
			Position = position;
			Normal = normal;
			DidCollide = true;
			RayDistance = rayDistance;
		}

		public RayCollisionPoint()
		{
			RayDistance = float.PositiveInfinity;
			DidCollide = false;
		}

		public Vector3 Position { get; }
		public Vector3 Normal { get; }
		public bool DidCollide { get; private set; }
		public float RayDistance { get; }
		public GameObject Collidable { get; }


		public void SetCollide(bool collide)
		{
			DidCollide = collide;
		}
	}
}