using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Code
{
	/// <summary>
	/// Improves kayboard controls with new functions.
	/// </summary>
	public static class KeyboardManager
	{
		private static Keys[] CurrentPressedKeys;
		private static Keys[] LastPressedKeys;

		public static void Update()
		{
			var ks = Keyboard.GetState();

			// Add all keys to the dictionary
			LastPressedKeys = CurrentPressedKeys;
			CurrentPressedKeys = ks.GetPressedKeys();
		}

		// Keyboardstate is just used for the extension method. 
		public static bool IsKeyDown(Keys key)
		{
			return CurrentPressedKeys.Contains(key) && !LastPressedKeys.Contains(key);
		}

		public static bool IsKeyDown(this KeyboardState _, Keys key) => IsKeyDown(key);

		// Keyboardstate is just used for the extension method. 
		public static bool IsKeyPressed(Keys key)
		{
			return CurrentPressedKeys.Contains(key);
		}
		public static bool IsKeyPressed(this KeyboardState _, Keys key) => IsKeyPressed(key);
	}
}
