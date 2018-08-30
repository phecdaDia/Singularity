using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Core.Utilities
{
	/// <summary>
	///     Template for ScreenEffect (if you want to do it as a class)
	/// </summary>
	public abstract class ScreenEffect
	{
		/// <summary>
		///     Function which is Given to the Game to define the behavior of the Screen
		/// </summary>
		/// <param name="gameTime">Elapsed Time</param>
		/// <param name="screen">Texture of the Screen for information purposes</param>
		/// <returns>Data defining the behavior</returns>
		public abstract ScreenEffectData GetEffectData(GameTime gameTime, Texture2D screen);
	}
}