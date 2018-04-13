using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	/// <summary>
	/// A GameObject can be any object in a GameScene.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GameObject
	{
		
		private Model Model;					// Model of the entity. Is Null if the object shall not be rendered.
		protected Vector3 Position;				// Current position of the model
		protected Vector3 Rotation;				// Current rotation of the model
		protected float Scale;					// Scale of the model

		private GameObject ParentObject;		// Parent Object. This object will be in the ChildObjects of the Parent.
		private List<GameObject> ChildObjects;	// Child Objects

		private List<Action<GameScene, GameObject, GameTime>> ObjectScripts;

		public GameObject()
		{
			// Setting default values for all members
			this.Position = new Vector3();
			this.Rotation = new Vector3();
			this.Scale = 1.0f;

			this.ParentObject = null;
			this.ChildObjects = new List<GameObject>();

			this.ObjectScripts = new List<Action<GameScene, GameObject, GameTime>>();
		}

		// Methods for the builder pattern

		#region Builder Pattern
		public GameObject SetModel(Model model)
		{
			this.Model = model;
			return this;
		}
		public GameObject SetPosition(Vector3 position)
		{
			this.Position = position;
			return this;
		}
		public GameObject SetRotation(Vector3 rotation)
		{
			this.Rotation = rotation;
			return this;
		}
		public GameObject SetScale(float scale)
		{
			this.Scale = scale;
			return this;
		}

		public GameObject SetParent(GameObject parent)
		{
			this.ParentObject = parent;
			parent.ChildObjects.Add(this);

			return this;
		}

		public GameObject AddScript(Action<GameScene, GameObject, GameTime> script)
		{
			this.ObjectScripts.Add(script);

			return this;
		}

		public GameObject AddChild(GameObject child)
		{
			this.ChildObjects.Add(child);
			child.ParentObject = this;
			return this;
		}
		#endregion

		public float GetHierarchyScale()
		{
			if (this.ParentObject == null) return this.Scale;
			return this.Scale * this.ParentObject.GetHierarchyScale();
		}
		public Vector3 GetHierarchyPosition()
		{
			if (this.ParentObject == null) return this.Position;
			return this.Position + this.ParentObject.GetHierarchyPosition();
		}

		public List<Action<GameScene, GameObject, GameTime>> GetScripts()
		{
			return this.ObjectScripts;
		}

		#region Abstract Methods

		public void UpdateLogic(GameScene scene, GameTime gameTime)
		{
			Update(scene, gameTime);
			foreach (GameObject obj in this.ChildObjects) obj.UpdateLogic(scene, gameTime);
		}

		public abstract void Update(GameScene scene, GameTime gameTime);

		#endregion

		public void DrawLogic(GameScene scene, SpriteBatch spriteBatch)
		{
			Draw(scene, spriteBatch);
			foreach (GameObject obj in this.ChildObjects) obj.DrawLogic(scene, spriteBatch);
		}

		public void Draw(GameScene scene, SpriteBatch spriteBatch)
		{
			if (this.Model == null) return; // No model means it can't be rendered.

			// copy the scale of bones from the model to apply it later.
			var transformMatrices = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(transformMatrices);

			foreach (ModelMesh mesh in this.Model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					// calculating the full rotation of our object.
					Console.WriteLine($"POS: {this.GetHierarchyPosition().X} {this.GetHierarchyPosition().Y} {this.GetHierarchyPosition().Z}");

					Matrix totalRotation = Matrix.CreateRotationX(this.Rotation.X) * Matrix.CreateRotationY(this.Rotation.Y) * Matrix.CreateRotationZ(this.Rotation.Z);

					effect.World = Matrix.CreateScale(this.GetHierarchyScale()) 
					               * totalRotation 
					               * Matrix.CreateTranslation(this.GetHierarchyPosition()) 
					               * transformMatrices[mesh.ParentBone.Index];
					effect.View = scene.GetViewMatrix();
					effect.Projection = scene.GetViewMatrix();

					//effect.EnableDefaultLighting();
					//effect.LightingEnabled = true; // Turn on the lighting subsystem.

					//effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f); // some diffuse light
					//effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
					//effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
					//scene.AddLightningToEffect(effect);

					//effect.EmissiveColor = new Vector3(1, 0, 0); // Sets some strange emmissive lighting.  This just looks weird.

				}

				mesh.Draw();
			}

			foreach (GameObject obj in this.ChildObjects) obj.Draw(scene, spriteBatch);
		}

	}
}
