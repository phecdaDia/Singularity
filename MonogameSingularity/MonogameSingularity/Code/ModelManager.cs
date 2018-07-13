using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	public static class ModelManager
	{		
		private static readonly Dictionary<string, Texture2D> ModelTextureDictionary = new Dictionary<string, Texture2D>();
			

		/// <summary>
		///     Gets <seealso cref="Model" /> cached.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Model GetModel(string path)
		{
			if (String.IsNullOrEmpty(path)) return null;

			return SingularityGame.GetContentManager().Load<Model>(path);
		}

		public static Texture2D GetTexture(string path)
		{
			if (ModelTextureDictionary.ContainsKey(path)) return ModelTextureDictionary[path];

			ModelTextureDictionary[path] = ((BasicEffect) GetModel(path).Meshes[0].Effects[0]).Texture;

			return ModelTextureDictionary[path];
		}
	}
}