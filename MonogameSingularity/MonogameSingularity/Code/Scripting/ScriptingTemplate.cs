using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Scripting
{
	public abstract class ScriptingTemplate
	{
		public SingularityGame Game { get; private set; }

		public virtual void Init(SingularityGame game)
		{
			Game = game;
		}

		public abstract SceneSettings GetSettings();
		public abstract void AddGameObjects(List<GameObject> objectList);
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
