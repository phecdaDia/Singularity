using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Scripting
{
	/// <summary>
	/// Template for LoadingScreen
	/// </summary>
	public abstract class LoadingScreenTemplate
	{
		public SingularityGame Game { get; private set; }

		/// <summary>
		/// Gets called after Construction
		/// </summary>
		/// <param name="game"></param>
		public virtual void Init(SingularityGame game)
		{
			Game = game;
		}

		/// <summary>
		/// Add Objects for scene to objectList. Do not delete. just add
		/// </summary>
		/// <param name="objectList">List of Gameobjects</param>
		public abstract void AddGameObjects(List<GameObject> objectList);

		/// <summary>
		/// Add Lighting
		/// </summary>
		/// <param name="effect">BasicEffect</param>
		public abstract void AddLightningToEffect(BasicEffect effect);
	}
}
