using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity;

namespace SingularityTest.Scenes
{
	public abstract class LightGameScene : GameScene
	{

		private readonly RenderTarget2D ShadowTarget2D;

		private readonly Effect ShadowMapGenerateEffect;

		private Matrix SunProjectionMatrix;

		private Vector3 LightPosition;
		private Vector3 LightDirection;

		private Vector3 ShadowMapUp = Vector3.UnitX;

		protected LightGameScene(SingularityGame game, string sceneKey, int shadowMapQuality, int sceneSize = 16, int minPartition = 2, float precision = 0) : base(game, sceneKey, sceneSize, minPartition, precision)
		{
			this.ShadowTarget2D = new RenderTarget2D(game.GraphicsDevice, shadowMapQuality, shadowMapQuality, false, SurfaceFormat.Vector4, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
			this.ShadowMapGenerateEffect = game.Content.Load<Effect>("effects/GenerateShadowMap");

			this.SetLightDirection(new Vector3(0, -1, 0));

		}

		public void SetLightPosition(Vector3 position)
		{
			this.LightPosition = position;
		}
		public void SetLightDirection(Vector3 direction)
		{
			this.LightDirection = direction;
			this.ShadowMapUp = Vector3.Transform(direction,
				Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.PiOver2) *
				Matrix.CreateRotationZ(MathHelper.PiOver2));
		}

		public void SetProjectionMatrix(Matrix projectionMatrix)
		{
			this.SunProjectionMatrix = projectionMatrix;
		}

		public override void Draw(SpriteBatch spriteBatch, RenderTarget2D finalTarget)
		{

			Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
			Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			Game.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
			Game.GraphicsDevice.BlendState = BlendState.Opaque;
			Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			// get all objects in the world.
			var gameObjects = this.GetAllObjects();

			// first we want to draw the shadow map
			Game.GraphicsDevice.SetRenderTarget(this.ShadowTarget2D);


			spriteBatch.Begin(SpriteSortMode.FrontToBack);
			Game.GraphicsDevice.Clear(Color.Transparent);


			foreach (var go in gameObjects)
			{
				go.DrawLogicWithEffect(this, spriteBatch, ShadowMapGenerateEffect, ShadowMapEffectParameters, "GenerateShadowMap", GameObjectDrawMode.Model); // We don't want any 2d Stuff
			}

			//spriteBatch.Draw(ShadowTarget2D, new Rectangle(0, 0, 200, 200), Color.White);

			spriteBatch.End();

			Game.GraphicsDevice.SetRenderTarget(finalTarget);


			Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
			Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			Game.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
			Game.GraphicsDevice.BlendState = BlendState.Opaque;
			Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			spriteBatch.Begin(SpriteSortMode.FrontToBack);
			Game.GraphicsDevice.Clear(Color.Transparent);

			// Draw the entire scene with our shadowmap
			foreach (var go in gameObjects)
			{
				if (go.Effect == null)
					go.DrawLogicWithEffect(this, spriteBatch, ShadowMapGenerateEffect, ShadowMapEffectParameters, "ShadowScene", GameObjectDrawMode.All); // We don't want any 2d Stuff
				else
					go.Draw(this, this.GetViewMatrix(), this.GetProjectionMatrix());
			}


			spriteBatch.Draw(this.ShadowTarget2D, new Rectangle(0, 0, 256, 256), Color.White);

			spriteBatch.End();


		}

		private void ShadowMapEffectParameters(GameObject gameObject, Effect effect, Matrix[] transformMatrices, ModelMesh mesh, GameScene scene)
		{
			var world = transformMatrices[mesh.ParentBone.Index] * gameObject.ScaleMatrix * gameObject.RotationMatrix *
						Matrix.CreateTranslation(gameObject.GetHierarchyPosition());

			effect.Parameters["World"]?.SetValue(world);
			effect.Parameters["View"]?.SetValue(scene.GetViewMatrix());
			effect.Parameters["Projection"]?.SetValue(scene.GetProjectionMatrix());

			effect.Parameters["LightView"]?.SetValue(Matrix.CreateLookAt(this.LightPosition, this.LightPosition + this.LightDirection, this.ShadowMapUp));
			effect.Parameters["LightProjection"]?.SetValue(this.SunProjectionMatrix);

			effect.Parameters["LightDirection"]?.SetValue(this.LightDirection);
			effect.Parameters["CameraPosition"]?.SetValue(this.CameraPosition);

			effect.Parameters["AmbientLightColor"]?.SetValue(new Vector4(1.0f, 0.8f, 0.7f, 0.5f));
			effect.Parameters["AmbientLightIntensity"]?.SetValue(0.4f);

			effect.Parameters["DiffuseLightColor"]?.SetValue(Color.White.ToVector4());
			effect.Parameters["DiffuseLightIntensity"]?.SetValue(0.2f);

			effect.Parameters["ShadowMap"]?.SetValue((Texture2D)this.ShadowTarget2D);
			effect.Parameters["Texture"]?.SetValue(gameObject.Texture);

			effect.Parameters["DefaultColor"]?.SetValue(Color.White.ToVector4());
			effect.Parameters["UseTexture"].SetValue(gameObject.Texture == null ? 0 : 1);
		}
	}
}
