using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity;
using Singularity.GameObjects;

namespace SingularityTest.GameObjects
{
	public class LightTestScene : GameScene
	{
		private RenderTarget2D ShadowTarget2D;

		private Effect ShadowMapGenerateEffect;

		private Matrix SunViewMatrix;
		private Matrix SunProjectionMatrix;

		public LightTestScene(SingularityGame game) : base(game, "light", 8, 0, 0.0f)
		{
			this.ShadowTarget2D = new RenderTarget2D(game.GraphicsDevice, 4096, 4096);
			this.ShadowMapGenerateEffect = game.Content.Load<Effect>("effects/GenerateShadowMap");

			this.SunViewMatrix = Matrix.CreateLookAt(new Vector3(10, 10, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
			this.SunProjectionMatrix = Matrix.CreateOrthographic(40, 40, 0.01f, 50.0f);

			this.SetAbsoluteCamera(new Vector3(-20, 20, 20), new Vector3(0, 0, 0));

		}

		protected override void AddGameObjects(int entranceId)
		{
			float width = 40.0f;

			int i = 0;

			while (width >= 1.0f)
			{
				AddObject(new ModelObject("cubes/cube1")
					.SetPosition(0, -0.5f + i, 0)
					.SetScale(width, 1, width)
				);

			i++;
			width /= 1.2f;
			}

			AddObject(new ModelObject("cubes/cube1")
				.SetPosition(20, 20, 0)
				.SetScale(width, 1, width)
			);

			AddObject(new BasicCamera().Set3DEnabled(true).SetPosition(0, 10, 10));

		}

		public override void AddLightningToEffect(Effect effect)
		{}

		public override void Draw(SpriteBatch spriteBatch, RenderTarget2D finalTarget)
		{
			// get all objects in the world.
			var gameObjects = this.GetAllObjects(o => o.ParentObject == null);

			// first we want to draw the shadow map
			Game.GraphicsDevice.SetRenderTarget(this.ShadowTarget2D);


			spriteBatch.Begin(SpriteSortMode.FrontToBack);
			Game.GraphicsDevice.Clear(Color.Transparent);


			foreach (var go in gameObjects)
			{
				// set shadow map generation shader TODO put this somewhere else!!!!
				go.SetEffect(ShadowMapGenerateEffect, ShadowMapEffectParameters);

				go.Effect.CurrentTechnique = go.Effect.Techniques["GenerateShadowMap"];

				go.DrawLogic(this, spriteBatch, GameObjectDrawMode.Model); // We don't want any 2d Stuff
			}

			//spriteBatch.Draw(ImageManager.GetTexture2D("mandelbrot"), new Vector2(), Color.White);

			spriteBatch.End();

			Game.GraphicsDevice.SetRenderTarget(finalTarget);
			

			spriteBatch.Begin(SpriteSortMode.FrontToBack);
			Game.GraphicsDevice.Clear(Color.Transparent);

			// Draw the entire scene with our shadowmap
			foreach (var go in gameObjects)
			{
				go.Effect.CurrentTechnique = go.Effect.Techniques["ShadowScene"];

				go.DrawLogic(this, spriteBatch, GameObjectDrawMode.Model);
			}


			spriteBatch.Draw(this.ShadowTarget2D, new Rectangle(0, 0, 512, 512), Color.White);

			spriteBatch.End();


		}

		private void ShadowMapEffectParameters(GameObject gameObject, Effect effect, Matrix[] transformMatrices, ModelMesh mesh, GameScene scene)
		{
			var world = transformMatrices[mesh.ParentBone.Index] * gameObject.ScaleMatrix * gameObject.RotationMatrix *
			            Matrix.CreateTranslation(gameObject.GetHierarchyPosition());

			effect.Parameters["World"]?.SetValue(world);
			effect.Parameters["View"]?.SetValue(scene.GetViewMatrix());
			effect.Parameters["Projection"]?.SetValue(scene.GetProjectionMatrix());

			effect.Parameters["LightView"]?.SetValue(this.SunViewMatrix);
			effect.Parameters["LightProjection"]?.SetValue(this.SunProjectionMatrix);
			effect.Parameters["MaxClippingDistance"]?.SetValue(50.0f);

			effect.Parameters["ShadowMap"]?.SetValue((Texture2D) this.ShadowTarget2D);
		}
	}
}
