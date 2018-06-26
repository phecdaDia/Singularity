using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityTest.Scenes
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework.Input;
	using Singularity;
	using Singularity.GameObjects;

	public class ShaderTestScene : GameScene
	{
		public ShaderTestScene(SingularityGame game) : base(game, "shadertest")
		{
			this.SetCameraTarget(new Vector3(1, 0, 0));
			this.SetCameraPosition(new Vector3(0, 0, 0));
			this.SetCullingDistance(0.01f, 5.0f);
		}

		protected override void AddGameObjects(int entranceId)
		{
			AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
				{
					if (KeyboardManager.IsKeyPressed(Keys.Escape)) SceneManager.CloseScene();

					if (Mouse.GetState().LeftButton == ButtonState.Pressed)
					{
						this.SetCullingDistance(0.01f, this.MaximumCullingDistance + (float) arg3.ElapsedGameTime.TotalSeconds);
					}

					if (Mouse.GetState().RightButton == ButtonState.Pressed)
					{
						this.SetCullingDistance(0.01f, this.MaximumCullingDistance - (float)arg3.ElapsedGameTime.TotalSeconds);
					}
				}));

			AddObject(new ModelObject("sphere")
				.SetPosition(2.5f, 0, 0)
				.SetScale(0.15f)
				.SetEffect(Game.Content.Load<Effect>("effects/GenerateShadowMap"), AddEffectParameters)
			);
		}

		private void AddEffectParameters(GameObject obj, Effect effect, Matrix[] transformMatrices, ModelMesh mesh, GameScene scene)
		{
			var world = transformMatrices[mesh.ParentBone.Index] * obj.ScaleMatrix * obj.RotationMatrix *
						Matrix.CreateTranslation(obj.GetHierarchyPosition());

			effect.Parameters["World"].SetValue(world);
			effect.Parameters["View"].SetValue(scene.GetViewMatrix());
			effect.Parameters["Projection"].SetValue(scene.GetProjectionMatrix());

			//effect.Parameters["WorldViewProjection"].SetValue(world * scene.GetViewMatrix() * scene.GetProjectionMatrix());
		}

		//public override void AddLightningToEffect(Effect effect)
		//{
		//	effect.Parameters["clipMax"]?.SetValue(this.MaximumCullingDistance);
		//	//effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(1,0,0));
		//	//effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
		//	//effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
		//	//effect.Parameters["AmbientIntensity"].SetValue(0.3f);
		//}
	}
}
