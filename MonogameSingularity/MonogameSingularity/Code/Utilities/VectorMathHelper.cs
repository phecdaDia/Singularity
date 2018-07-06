using System;
using Microsoft.Xna.Framework;

namespace Singularity.Utilities
{
	public static class VectorMathHelper
	{
		/// <summary>
		///     Solved x = ua + vb + sc + td
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
			var equationMatrix = new Matrix(
				a, b, c, d
			);

			// not an error? *insert shrug emote*
			//if (equationMatrix.Determinant() < float.Epsilon) throw new ArgumentException("Deteminant of Matrix is 0");

			var solution = Vector4.Transform(x, Matrix.Invert(equationMatrix));

			// divide each parameter by the square length of the start vector
			//solution.X /= Vector4.Dot(a, a);
			//solution.Y /= Vector4.Dot(b, b);
			//solution.Z /= Vector4.Dot(c, c);
			//solution.W /= Vector4.Dot(d, d);

			//Console.WriteLine($"{solution}");

			//Console.WriteLine($"B: {x} => {solution}");
			//Console.WriteLine($"Merged: {solution.X * a + solution.Y * b + solution.Z * c + solution.W * d}");

			return solution;
		}


		/// <summary>
		///     Solved x = ua + vb + sc
		/// </summary>
		/// <returns></returns>
		public static Vector3 SolveLinearEquation(Vector3 a, Vector3 b, Vector3 c, Vector3 x)
		{
			var v4solution = SolveLinearEquation(
				new Vector4(a, 0),
				new Vector4(b, 0),
				new Vector4(c, 0),
				new Vector4(0, 0, 0, 1),
				new Vector4(x, 1)
			);

			return new Vector3(v4solution.X, v4solution.Y, v4solution.Z);
		}


		/// <summary>
		///     Solved x = ua + vb
		/// </summary>
		/// <returns></returns>
		public static Vector2 SolveLinearEquation(Vector2 a, Vector2 b, Vector2 x)
		{
			var v4solution = SolveLinearEquation(
				new Vector4(a, 0, 0),
				new Vector4(b, 0, 0),
				new Vector4(0, 0, 1, 0),
				new Vector4(0, 0, 0, 1),
				new Vector4(x, 1, 1)
			);

			return new Vector2(v4solution.X, v4solution.Y);
		}
	}
}