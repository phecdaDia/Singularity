using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Scripting
{
	/// <summary>
	/// Template for scripting
	/// </summary>
	public abstract class ScriptingTemplate
	{
		public SingularityGame Game { get; private set; }

		/// <summary>
		/// Initialize - gets called after construction
		/// </summary>
		/// <param name="game">SingularityGame</param>
		public virtual void Init(SingularityGame game)
		{
			Game = game;
		}

		/// <summary>
		/// Return Settings for setting up Scene - if a value is left null it will use default
		/// </summary>
		/// <returns>Settings of Scene</returns>
		public abstract SceneSettings GetSettings();

		/// <summary>
		/// Add all GameObjects for this scene to objectList
		/// </summary>
		/// <param name="objectList">a complete instanciated objectList. don't delete. just add</param>
		public abstract void AddGameObjects(List<GameObject> objectList);

		/// <summary>
		/// Define lighting
		/// </summary>
		/// <param name="effect">BasicEffect</param>
		public abstract void AddLightningToEffect(BasicEffect effect);
	}

	/// <summary>
	/// Settings. Set null for default
	/// </summary>
	public struct SceneSettings
	{
		public string SceneKey;
		public int? SceneSize;
		public int? MinPartition;
		public float? Precision;
	}
}
