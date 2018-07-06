using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	public class ModelManager
	{
		private static ModelManager Instance;

		private readonly ContentManager ContentManager;
		private readonly Dictionary<string, Model> ModelDictionary;
		private readonly Dictionary<string, Texture2D> ModelTextureDictionary;


		public ModelManager(ContentManager contentManager)
		{
			if (Instance != null) return;
			Instance = this;

			ContentManager = contentManager;
			ModelDictionary = new Dictionary<string, Model>();
			ModelTextureDictionary = new Dictionary<string, Texture2D>();
		}

		/// <summary>
		///     Gets the singleton instance
		/// </summary>
		/// <returns></returns>
		private static ModelManager GetInstance()
		{
			return Instance;
		}

		/// <summary>
		///     Gets <seealso cref="Model" /> cached.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private Model _GetModel(string path)
		{
			if (ModelDictionary.ContainsKey(path)) return ModelDictionary[path];

			var model = ContentManager.Load<Model>(path);
			ModelDictionary[path] = model;

			ModelTextureDictionary[path] = ((BasicEffect) model.Meshes[0].Effects[0]).Texture;


			return model;
		}

		private Texture2D _GetTexture(string path)
		{
			if (ModelTextureDictionary.ContainsKey(path)) return ModelTextureDictionary[path];

			_GetModel(path);
			return ModelTextureDictionary[path];
		}

		/// <summary>
		///     Gets <seealso cref="Model" /> cached.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Model GetModel(string path)
		{
			return GetInstance()._GetModel(path);
		}

		public static Texture2D GetTexture(string path)
		{
			return GetInstance()._GetTexture(path);
		}
	}
}