using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.GameObjects;

namespace SingularityTest.Scenes
{
	public class RayTestScene : GameScene
	{
		private int LastMouseX, LastMouseY;

		public RayTestScene(SingularityGame game, string sceneKey) : base(game, sceneKey, 8, 2, 0.0f)
		{
			this.ScenePauseEvent += (sender, args) =>
			{
				this.LastMouseX = Mouse.GetState().X;
				this.LastMouseY = Mouse.GetState().Y;
			};
			this.SceneResumeEvent += (sender, args) => Mouse.SetPosition(LastMouseX, LastMouseY);
		}

		protected override void AddGameObjects(int entranceId)
		{
			AddObject(new ModelObject("cubes/cube1").SetPosition(-10.5f, 0, 0).SetScale(10, 1, 10));


			
			SetAbsoluteCamera(new Vector3(), new Vector3(-10, 0, 0));
		}

		public override void AddLightningToEffect(BasicEffect effect)
		{
			
		}
	}
}
