using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Core;
using Singularity.Core.GameObjects;
using Singularity.Examples.GameObjects;

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

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			AddObject(new GameObject().AddScript((scene, o, arg3) =>
			{
				if (KeyboardManager.IsKeyPressed(Keys.Escape)) SceneManager.CloseScene();

				if (Mouse.GetState().LeftButton == ButtonState.Pressed)
					SetCullingDistance(0.01f, MaximumCullingDistance + (float) arg3.ElapsedGameTime.TotalSeconds);

				if (Mouse.GetState().RightButton == ButtonState.Pressed)
					SetCullingDistance(0.01f, MaximumCullingDistance - (float) arg3.ElapsedGameTime.TotalSeconds);
			}));

			AddObject(new ModelObject("sphere")
				.SetPosition(2.5f, 0, 0)
				.SetScale(1f)
				.SetEffect(Game.Content.Load<Effect>("effects/TestShader"), AddEffectParameters)
			);
		}

		private void AddEffectParameters(GameObject obj, Effect effect, Matrix[] transformMatrices, ModelMesh mesh,
			GameScene scene)
		{
			effect.Parameters["WorldViewProjection"].SetValue(obj.WorldMatrix * scene.GetViewMatrix() * scene.GetProjectionMatrix());
		}
	}
}