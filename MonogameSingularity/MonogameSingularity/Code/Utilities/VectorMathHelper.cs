using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Utilities
{
	public static class VectorMathHelper
	{
		/// <summary>
		/// Solved x = ua + vb + sc + td
		/// </summary>
		/// <returns></returns>
		public static Vector4 SolveLinearEquation(Vector4 a, Vector4 b, Vector4 c, Vector4 d, Vector4 x)
		{
			// check if this equation system is solvable

			if (a.LengthSquared() < float.Epsilon) throw new ArgumentException("Vector a is of length 0");
			if (b.LengthSquared() < float.Epsilon) throw new ArgumentException("Vector b is of length 0");
			if (c.LengthSquared() < float.Epsilon) throw new ArgumentException("Vector c is of length 0");
			if (d.LengthSquared() < float.Epsilon) throw new ArgumentException("Vector d is of length 0");

			// build the Matrix A to transform this into Ab = x 
			Matrix equationMatrix = new Matrix(
				a.X, b.X, c.X, d.X,
				a.Y, b.Y, c.Y, d.Y,
				a.Z, b.Z, c.Z, d.Z,
				a.W, b.W, c.W, d.W
			);

			if (equationMatrix.Determinant() < float.Epsilon) throw new ArgumentException("Deteminant of Matrix is 0");

			Vector4 solution = Vector4.Transform(x, equationMatrix);

			// divide each parameter by the square length of the start vector
			solution.X /= a.LengthSquared();
			solution.Y /= b.LengthSquared();
			solution.Z /= c.LengthSquared();
			solution.W /= d.LengthSquared();

			return solution;
		}


		/// <summary>
		/// Solved x = ua + vb + sc
		/// </summary>
		/// <returns></returns>
		public static Vector3 SolveLinearEquation(Vector3 a, Vector3 b, Vector3 c, Vector3 x)
		{
			Vector4 v4solution = SolveLinearEquation(
				new Vector4(a, 0),
				new Vector4(b, 0),
				new Vector4(c, 0),
				new Vector4(0, 0, 0, 1),
				new Vector4(x, 0)
			);

			return new Vector3(v4solution.X, v4solution.Y, v4solution.Z);
		}


		/// <summary>
		/// Solved x = ua + vb
		/// </summary>
		/// <returns></returns>
		public static Vector2 SolveLinearEquation(Vector2 a, Vector2 b, Vector2 x)
		{
			Vector4 v4solution = SolveLinearEquation(
				new Vector4(a, 0, 0),
				new Vector4(b, 0, 0),
				new Vector4(0, 0, 1, 0),
				new Vector4(0, 0, 0, 1),
				new Vector4(x, 0, 0)
			);

			return new Vector2(v4solution.X, v4solution.Y);
		}
	}
}
