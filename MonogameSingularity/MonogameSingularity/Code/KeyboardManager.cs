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
		private static List<Keys> CurrentPressedKeys = new List<Keys>();
		private static List<Keys> LastPressedKeys = new List<Keys>();

		/// <summary>
		/// Updates
		/// </summary>
		public static void Update()
		{
			var ks = Keyboard.GetState();

			// Add all keys to the dictionary
			LastPressedKeys = CurrentPressedKeys;
			CurrentPressedKeys = ks.GetPressedKeys().ToList();
		}

		#region IsKeyPressed
		
		/// <summary>
		/// Returns true, if the <seealso cref="Keys"/> have been pressed this frame, but not last one.
		/// Only returns true, if the <seealso cref="Keys"/> have been pressed this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyPressed(Keys key)
		{
			return CurrentPressedKeys.Contains(key) && !LastPressedKeys.Contains(key);
		}
		
		public static bool IsKeyPressed(this KeyboardState _, Keys key) => IsKeyPressed(key);

		#endregion

		#region IsKeyReleased

		/// <summary>
		/// Returns true, if the <seealso cref="Keys"/> have been released this frame, but not last one.
		/// Only returns true, if the <seealso cref="Keys"/> have been released this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyReleased(Keys key)
		{
			return !CurrentPressedKeys.Contains(key) && LastPressedKeys.Contains(key);
		}

		public static bool IsKeyReleased(this KeyboardState _, Keys key) => IsKeyReleased(key);

		#endregion

		#region IsKeyDown
		
		/// <summary>
		/// Returns true, if the <seealso cref="Keys"/> have been pressed this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyDown(Keys key)
		{
			return CurrentPressedKeys.Contains(key);
		}
		public static bool IsKeyDown(this KeyboardState _, Keys key) => IsKeyDown(key);

		#endregion

		#region IsKeyUp

        /// <summary>
		/// Returns true, if the <seealso cref="Keys"/> have not been pressed this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
	    public static bool IsKeyUp(Keys key)
	    {
	        return !CurrentPressedKeys.Contains(key);
	    }
	    public static bool IsKeyUp(this KeyboardState _, Keys key) => IsKeyUp(key);

	    #endregion

	}
}
