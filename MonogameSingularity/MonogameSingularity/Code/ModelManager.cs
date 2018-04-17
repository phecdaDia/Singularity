using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	public class ModelManager
	{
		private static ModelManager Instance;

		private readonly ContentManager ContentManager;
		private readonly Dictionary<String, Model> ModelDictionary;


		public ModelManager(ContentManager contentManager)
		{
			if (Instance != null) return;
			Instance = this;

			this.ContentManager = contentManager;
			this.ModelDictionary = new Dictionary<string, Model>();
		}

		public static ModelManager GetInstance()
		{
			return Instance;
		}

		private Model _GetModel(String path)
		{
			if (this.ModelDictionary.ContainsKey(path)) return this.ModelDictionary[path];

			Model model = ContentManager.Load<Model>(path);
			this.ModelDictionary[path] = model;

			return model;
		}

		public static Model GetModel(String path)
		{
			return GetInstance()._GetModel(path);
		}
	}
}
