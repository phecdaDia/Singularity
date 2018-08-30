using Microsoft.Xna.Framework;

namespace Singularity.Core.Collisions
{
	public class PlaneCollision : Collision
	{
		protected readonly Vector3 _origin;


		protected readonly Vector3 _spanVector1;


		protected readonly Vector3 _spanVector2;


		protected Vector3 _normal;

		public PlaneCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2)
		{
			this._origin = origin;
			this._spanVector1 = spanVector1;
			this._spanVector2 = spanVector2;
			this._normal = Vector3.Cross(spanVector1, spanVector2);

			this._normal.Normalize();
		}

		public Vector3 Origin
		{
			get { return Vector3.Transform(this._origin, this.Parent.TransformationMatrix) + this.Parent.GetHierarchyPosition(); }
		}

		public Vector3 SpanVector1
		{
			get { return Vector3.Transform(this._spanVector1, this.Parent.TransformationMatrix); }
		}

		public Vector3 SpanVector2
		{
			get { return Vector3.Transform(this._spanVector2, this.Parent.TransformationMatrix); }
		}

		public Vector3 Normal
		{
			get { return Vector3.Transform(this._normal, this.Parent.TransformationMatrix); }
		}

		public override object Clone()
		{
			return new PlaneCollision(this._origin, this._spanVector1, this._spanVector2);
		}
	}
}