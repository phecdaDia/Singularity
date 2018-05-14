using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Scripting
{
	using Microsoft.Xna.Framework.Graphics;

	using Utilities;

	public class ScriptLoadingScene : GameScene
	{
		/// <summary>
		/// Creates a new <see cref="GameScene"/>
		/// </summary>
		/// <param name="sceneKey">Unique key for <seealso cref="SceneManager"/></param>
		/// <param name="sceneSize">Size of the scene in 2^x</param>
		/// <param name="minPartition">Minimum size of <seealso cref="Octree{T}"/> partitioning</param>
		/// <param name="precision">Buffer radius for <seealso cref="Octree{T}"/></param>
		public ScriptLoadingScene(SingularityGame game, string sceneKey, string pathToLoadingScript ,int sceneSize = 16, int minPartition = 2, float precision = 0) : base(game, sceneKey, sceneSize, minPartition, precision)
		{

		}

		/// <summary>
		/// Adds all <seealso cref="GameObject"/>
		/// </summary>
		protected override void AddGameObjects()
		{

		}

		/// <summary>
		/// Adds lightning to the <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="effect"></param>
		public override void AddLightningToEffect(BasicEffect effect)
		{
		}
	}
}
