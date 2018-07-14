using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	/// <summary>
	///     Loads <see cref="Texture2D" /> from the ContentPipeline
	/// </summary>
	public static class ImageManager
	{

		/// <summary>
		///     Returns the <see cref="Texture2D" /> and if not yet cached in <see cref="TextureDictionary" /> calls
		///     <see cref="LoadTexture(string)" />
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[Obsolete("Use SingularityGame.GetContentManager() to get access to the contentmanager", true)]
		public static Texture2D GetTexture2D(string key)
		{

			return SingularityGame.GetContentManager().Load<Texture2D>(key);
		}
	}
}