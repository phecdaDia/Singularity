using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.GameObjects;

namespace SingularityTest.Scenes
{
	public class ShaderTestScene : GameScene
	{
		public ShaderTestScene(SingularityGame game) : base(game, "shadertest")
		{
			SetCameraTarget(new Vector3(1, 0, 0));
			SetCameraPosition(new Vector3(0, 0, 0));
			SetCullingDistance(0.01f, 5.0f);
		}

		protected override void AddGameObjects(int entranceId)
		{
			AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.Escape)) SceneManager.CloseScene();

				if (Mouse.GetState().LeftButton == ButtonState.Pressed)
					SetCullingDistance(0.01f, MaximumCullingDistance + (float) arg3.ElapsedGameTime.TotalSeconds);

				if (Mouse.GetState().RightButton == ButtonState.Pressed)
					SetCullingDistance(0.01f, MaximumCullingDistance - (float) arg3.ElapsedGameTime.TotalSeconds);
			}));

			AddObject(new ModelObject("sphere")
				.SetPosition(2.5f, 0, 0)
				.SetScale(0.15f)
				.SetEffect(Game.Content.Load<Effect>("effects/GenerateShadowMap"), AddEffectParameters)
			);
		}

		private void AddEffectParameters(GameObject obj, Effect effect, Matrix[] transformMatrices, ModelMesh mesh,
			GameScene scene)
		{
			var world = transformMatrices[mesh.ParentBone.Index] * obj.ScaleMatrix * obj.RotationMatrix *
			            Matrix.CreateTranslation(obj.GetHierarchyPosition());

			effect.Parameters["World"].SetValue(world);
			effect.Parameters["View"].SetValue(scene.GetViewMatrix());
			effect.Parameters["Projection"].SetValue(scene.GetProjectionMatrix());
		}
	}
}