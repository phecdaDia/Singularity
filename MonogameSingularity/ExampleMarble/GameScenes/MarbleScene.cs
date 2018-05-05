using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleMarble.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code;
using Singularity.Code.GameObjects;

namespace ExampleMarble.GameScenes
{
	public class MarbleScene : GameScene
	{
		public MarbleScene(SingularityGame game) : base(game, "marble", 10, 0, 0.1f)
		{
			
		}

		protected override void AddGameObjects()
		{
			// build a nice maze and add a player. 
			AddObject(new MarbleObject().SetPosition(0, 0, 10)); // adds player
		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			//effect.DirectionalLight0.Enabled = true;
			//effect.DirectionalLight0.DiffuseColor = new Vector3(0.55f, 0.55f, 0.75f); // a bit more blue than the other.
			//effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);
			//effect.SpecularPower = 0.05f;
			//effect.SpecularColor = new Vector3(0.05f, 0.05f, 0.1f);
			effect.EnableDefaultLighting(); 

		}
	}
}
