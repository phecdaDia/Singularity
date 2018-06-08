using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class PlaneCollision : Collision
	{
		protected readonly Vector3 _origin;
		public Vector3 Origin
		{
			get { return Vector3.Transform(_origin, this.Parent.TransformationMatrix) + this.Parent.GetHierarchyPosition(); }
		}


		protected readonly Vector3 _spanVector1;
		public Vector3 SpanVector1
		{
			get { return Vector3.Transform(_spanVector1, this.Parent.TransformationMatrix); }
		}


		protected readonly Vector3 _spanVector2;
		public Vector3 SpanVector2
		{
			get { return Vector3.Transform(_spanVector2, this.Parent.TransformationMatrix); }
		}


		protected Vector3 _normal;
		public Vector3 Normal
		{
			get { return Vector3.Transform(_normal, this.Parent.TransformationMatrix); }
		}

		public PlaneCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2) : base()
		{
			_origin = origin;
			_spanVector1 = spanVector1;
			_spanVector2 = spanVector2;
			_normal = Vector3.Cross(spanVector1, spanVector2);

			_normal.Normalize();
		}

		public override object Clone()
		{
			return new PlaneCollision(this.Origin, this._spanVector1, this._spanVector2);
		}
	}
}
