using System;
using Microsoft.Xna.Framework;

namespace Singularity.Helpers
{
	public static class VectorHelper
	{
		public static Vector2 GetScreenPosition(this Vector3 position, Matrix viewMatrix, Matrix projectionMatrix,
			int screenWidth,
			int screenHeight)
		{
			var screenPosition = new Vector4(position, 1);

			// fire through our camera matrices. 
			screenPosition = Vector4.Transform(screenPosition, viewMatrix);
			screenPosition = Vector4.Transform(screenPosition, projectionMatrix);

			// prevent divion by 0
			if (Math.Abs(screenPosition.W) > float.Epsilon) screenPosition /= Math.Abs(screenPosition.W);

			//Console.WriteLine($"{screenPosition}");

			return new Vector2(screenWidth / 2f * screenPosition.X + screenWidth / 2f,
				screenHeight / 2f * -screenPosition.Y + screenHeight / 2f);
		}
	}
}