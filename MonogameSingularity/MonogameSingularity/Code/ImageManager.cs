using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	/// <summary>
	///     Loads <see cref="Texture2D" /> from the ContentPipeline
	/// </summary>
	public class ImageManager
	{
		/// <summary>
		///     Singleton Instance
		/// </summary>
		private static ImageManager Instance;

		/// <summary>
		///     Provided ContentPipeline
		/// </summary>
		private static ContentManager Content;

		/// <summary>
		///     Cached <see cref="Dictionary{string, Texture2D}" />
		/// </summary>
		private readonly Dictionary<string, Texture2D> TextureDictionary;

		// Creating the ImageManager
		private ImageManager()
		{
			// ImageManager is a singleton. If there already is an Instance, we don't want to create another one.
			if (Instance != null) return;

			// Setting the singleton Instance
			Instance = this;

			// Creating the TextureDirectory
			TextureDictionary = new Dictionary<string, Texture2D>();
		}

		/// <summary>
		///     Sets the <see cref="ContentManager" />
		/// </summary>
		/// <param name="contentManager"></param>
		public static void SetContentManager(ContentManager contentManager)
		{
			Content = contentManager;
		}

		/// <summary>
		///     Returns Singleton Instance
		/// </summary>
		/// <returns></returns>
		private static ImageManager GetInstance()
		{
			if (Content == null) return null;

			return Instance ?? (Instance = new ImageManager());
		}

		/// <summary>
		///     Loads <see cref="Texture2D" /> by <paramref name="key" />
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private Texture2D LoadTexture(string key)
		{
			try
			{
				// Loading the texture
				var texture = Content.Load<Texture2D>(key);

				// Adding texture to our TextureDictionary
				TextureDictionary.Add(key, texture);

				// return texture if the texture was loaded successfully. 
				return texture;
			}
			catch (Exception /*ex*/)
			{
				// Teture could not be loaded. Returning null. 
				return null;
			}
		}

		/// <summary>
		///     Returns the <see cref="Texture2D" /> and if not yet cached in <see cref="TextureDictionary" /> calls
		///     <see cref="LoadTexture(string)" />
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static Texture2D GetTexture2D(string key)
		{
			// We have to create "output" here. CSharp 2015 doesn't like it in the out.

			GetInstance().TextureDictionary.TryGetValue(key, out var output);

			// output is null when the image wasn't loaded yet.
			if (output != null) return output;

			// Loading the texture from the pipeline
			output = GetInstance().LoadTexture(key);

			if (output != null) return output;


			// Image was not found in content pipeline
			// Fallback image is used
			Console.WriteLine("[ImageManager] Texture2D could not be loaded: {0}", key);
			Console.WriteLine("\tUsing fallback texture");
			GetInstance().TextureDictionary.TryGetValue("fallback", out output);

			return output;
		}
	}
}