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

			var model = SingularityGame.GetContentManager().Load<Model>(path);
			

			if (!ModelTextureDictionary.ContainsKey(path)) LoadTexture(model, path);

			return model;
		}

		public static Texture2D GetTexture(string path)
		{
			if (ModelTextureDictionary.ContainsKey(path)) return ModelTextureDictionary[path];

			GetModel(path);

			return ModelTextureDictionary[path];
		}

		private static void LoadTexture(Model model, string path)
		{
			ModelTextureDictionary[path] = ((BasicEffect)model.Meshes[0].Effects[0]).Texture;
		}
	}
}