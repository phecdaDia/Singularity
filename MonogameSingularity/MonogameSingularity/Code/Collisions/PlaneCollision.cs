using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class PlaneCollision : Collision
	{
		protected readonly Vector3 _origin;


		protected readonly Vector3 _spanVector1;


		protected readonly Vector3 _spanVector2;


		protected Vector3 _normal;

		public PlaneCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2)
		{
			_origin = origin;
			_spanVector1 = spanVector1;
			_spanVector2 = spanVector2;
			_normal = Vector3.Cross(spanVector1, spanVector2);

			_normal.Normalize();
		}

		public Vector3 Origin
		{
			get { return Vector3.Transform(_origin, Parent.TransformationMatrix) + Parent.GetHierarchyPosition(); }
		}

		public Vector3 SpanVector1
		{
			get { return Vector3.Transform(_spanVector1, Parent.TransformationMatrix); }
		}

		public Vector3 SpanVector2
		{
			get { return Vector3.Transform(_spanVector2, Parent.TransformationMatrix); }
		}

		public Vector3 Normal
		{
		    get
		    {
		        var normal2 = Vector3.Transform(_normal, Parent.TransformationMatrix);
		        return normal2 / normal2.Length();
		    }
		}

		public override object Clone()
		{
			return new PlaneCollision(_origin, _spanVector1, _spanVector2);
		}
	}
}