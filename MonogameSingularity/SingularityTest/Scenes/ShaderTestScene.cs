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
			this.SetAbsoluteCameraTarget(new Vector3());
			this.SetCameraPosition(new Vector3(5, 5, 5));
		}

		protected override void AddGameObjects(int entranceId)
		{
			AddObject(new EmptyGameObject().AddScript((scene, o, arg3) =>
			                                           {
				                                           if (KeyboardManager.IsKeyPressed(Keys.Escape)) SceneManager.CloseScene();
			                                           }));

			AddObject(new ModelObject("cubes/cube2")
			          .SetEffect(Game.Content.Load<Effect>("effects/Diffuse"), AddEffectParameters)
			          .AddScript((scene, o, arg3) =>
			                      {
				                      o.AddRotation(MathHelper.TwoPi * ((float)arg3.ElapsedGameTime.TotalSeconds/2f), 0, 0);
								  }));
		}

		private void AddEffectParameters(GameObject obj, Effect effect, Matrix[] transformMatrices, ModelMesh mesh, GameScene scene)
		{
			var world = transformMatrices[mesh.ParentBone.Index] * obj.ScaleMatrix * obj.RotationMatrix *
			            Matrix.CreateTranslation(obj.GetHierarchyPosition());

			effect.Parameters["World"].SetValue(world);
			effect.Parameters["View"].SetValue(scene.GetViewMatrix());
			effect.Parameters["Projection"].SetValue(scene.GetProjectionMatrix());

			var worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world));
			effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
		}

		public override void AddLightningToEffect(Effect effect)
		{
			effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(1,0,0));
			effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
			effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
			effect.Parameters["AmbientIntensity"].SetValue(0.3f);
		}
	}
}
