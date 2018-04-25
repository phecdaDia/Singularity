using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code.Enum;

namespace Singularity.Code
{
	/// <summary>
	/// A GameObject can be any object in a GameScene.
	/// </summary>
	public abstract class GameObject
	{
		
		public Model Model { get; private set; }// Model of the entity. Is Null if the object shall not be rendered.
		public Vector3 Position { get; private set; }// Current position of the model
		public Vector3 Rotation { get; private set; }// Current rotation of the model
		public Vector3 Scale { get; private set; }// Scale of the model


		public GameObject ParentObject { get; private set; }// Parent Object. This object will be in the ChildObjects of the Parent.
		public List<GameObject> ChildObjects { get; private set; }// Child Objects

		public Vector3 BoundingBoxMin { get; private set; }
		public Vector3 BoundingBoxMax { get; private set; }

		public String DebugName { get; private set; }				// Used for debugging.

		private List<Action<GameScene, GameObject, GameTime>> ObjectScripts;

		public CollisionMode CollisionMode { get; private set; }

		public GameObject()
		{
			// Setting default values for all members
			this.Position = new Vector3();
			this.Rotation = new Vector3();
			this.Scale = Vector3.One;

			this.ParentObject = null;
			this.ChildObjects = new List<GameObject>();

			this.ObjectScripts = new List<Action<GameScene, GameObject, GameTime>>();

			this.CollisionMode = CollisionMode.BoundingSphere;
		}

		// Methods for the builder pattern

		#region Builder Pattern
		public GameObject SetModel(Model model)
		{
			this.Model = model;
			return this;
		}

		public GameObject SetPosition(float x, float y) => SetPosition(new Vector3(x, y, 0));
		public GameObject SetPosition(float x, float y, float z) => SetPosition(new Vector3(x, y, z));
		public GameObject SetPosition(Vector3 position)
		{
			this.Position = position;
			return this;
		}

		public GameObject AddPosition(float x, float y) => AddPosition(new Vector3(x, y, 0));
		public GameObject AddPosition(float x, float y, float z) => AddPosition(new Vector3(x, y, z));
		public GameObject AddPosition(Vector3 position)
		{
			this.Position += position;
			return this;
		}

		public GameObject SetRotation(float x, float y) => SetRotation(new Vector3(x, y, 0));
		public GameObject SetRotation(float x, float y, float z) => SetRotation(new Vector3(x, y, z));
		public GameObject SetRotation(Vector3 rotation)
		{
			this.Rotation = rotation;
			return this;
		}


		public GameObject AddRotation(float x, float y) => AddRotation(new Vector3(x, y, 0));
		public GameObject AddRotation(float x, float y, float z) => AddRotation(new Vector3(x, y, z));
		public GameObject AddRotation(Vector3 rotation)
		{
			this.Rotation += rotation;
			return this;
		}

		public GameObject SetScale(float scale) => SetScale(scale, scale, scale);
		public GameObject SetScale(float x, float y, float z) => SetScale(new Vector3(x, y, z));
		public GameObject SetScale(Vector3 scale)
		{
			this.Scale = scale;
			return this;
		}

		public GameObject MultiplyScale(float x, float y, float z) => MultiplyScale(new Vector3(x, y, z));
		public GameObject MultiplyScale(Vector3 scale)
		{
			this.Scale *= scale;
			return this;
		}

		public GameObject AddScale(float scale) => AddScale(new Vector3(scale));
		public GameObject AddScale(float x, float y, float z) => AddScale(new Vector3(x, y, z));
		public GameObject AddScale(Vector3 scale)
		{
			this.Scale += scale;
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

		public GameObject SetCollisionMode(CollisionMode collisionMode)
		{
			this.CollisionMode = collisionMode;

			if (this.CollisionMode == CollisionMode.BoundingBox)
			{
				SetBoundingBox();
			}

			return this;
		}

		public GameObject SetDebugName(String name)
		{
			this.DebugName = name;
			return this;
		}
		#endregion

		public Vector3 GetHierarchyScale()
		{
			if (this.ParentObject == null) return this.Scale;
			return this.Scale * this.ParentObject.GetHierarchyScale();
		}
		public Vector3 GetHierarchyPosition()
		{
			if (this.ParentObject == null) return this.Position;
			return this.Position + this.ParentObject.GetHierarchyPosition();
		}

		// By https://pastebin.com/47vwJWSc
		public BoundingBox GetBoundingBox()
		{

			if (this.CollisionMode != CollisionMode.BoundingBox) return new BoundingBox();
			

			return GetBoundingBox(
				this.Model, 
				Matrix.CreateScale(this.GetHierarchyScale()) *
				Matrix.CreateRotationX(this.Rotation.X) * Matrix.CreateRotationY(this.Rotation.Y) * Matrix.CreateRotationZ(this.Rotation.Z)
			);
		}

		private void SetBoundingBox()
		{
			if (this.Model == null) return;

			//Matrix transform = Matrix.CreateRotationX(this.Rotation.X) * 
			//                   Matrix.CreateRotationY(this.Rotation.Y) *
			//                   Matrix.CreateRotationZ(this.Rotation.Z);

			// Initialize minimum and maximum corners of the bounding box to max and min values
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			// For each mesh of the model
			foreach (ModelMesh mesh in this.Model.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					// Vertex buffer parameters
					int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
					int vertexBufferSize = meshPart.NumVertices * vertexStride;

					// Get vertex data as float
					float[] vertexData = new float[vertexBufferSize / sizeof(float)];
					meshPart.VertexBuffer.GetData<float>(vertexData);

					// Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
					for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
					{
						Vector3 transformedPosition = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);

						min = Vector3.Min(min, transformedPosition);
						max = Vector3.Max(max, transformedPosition);
					}
				}
			}

			this.BoundingBoxMin = min;
			this.BoundingBoxMax = max;

		}

		public static BoundingBox GetBoundingBox(Model model, Matrix worldTransformation)
		{

			if (model == null) return new BoundingBox();

			// Initialize minimum and maximum corners of the bounding box to max and min values
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			// For each mesh of the model
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					// Vertex buffer parameters
					int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
					int vertexBufferSize = meshPart.NumVertices * vertexStride;

					// Get vertex data as float
					float[] vertexData = new float[vertexBufferSize / sizeof(float)];
					meshPart.VertexBuffer.GetData<float>(vertexData);

					// Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
					for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
					{
						Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransformation);

						min = Vector3.Min(min, transformedPosition);
						max = Vector3.Max(max, transformedPosition);
					}
				}
			}

			// Create and return bounding box
			return new BoundingBox(min, max);
		}

		public Boolean DoesCollide(BoundingSphere bs)
		{
			if (this.Model == null) return false;

			var world = Matrix.CreateScale(this.GetHierarchyScale()) * 
						Matrix.CreateRotationX(this.Rotation.X) *
			            Matrix.CreateRotationY(this.Rotation.Y) *
			            Matrix.CreateRotationZ(this.Rotation.Z) *
			            Matrix.CreateTranslation(this.GetHierarchyPosition());

			if (this.CollisionMode == CollisionMode.BoundingBox)
			{
				var min = Vector3.Transform(this.BoundingBoxMin, world);
				var max = Vector3.Transform(this.BoundingBoxMax, world);
				
				return bs.Intersects(new BoundingBox(min, max));
			}

			if (this.CollisionMode == CollisionMode.BoundingSphere)
			{
				foreach (var mesh in this.Model.Meshes)
				{
					if (mesh.BoundingSphere.Transform(world).Intersects(bs)) return true;
				}

				return false;
			}
			return false;
		}

		public List<Action<GameScene, GameObject, GameTime>> GetScripts()
		{
			return this.ObjectScripts;
		}

		#region Abstract Methods

		public void UpdateLogic(GameScene scene, GameTime gameTime)
		{
			// get a copy of the position
			var position = this.GetHierarchyPosition();

			Update(scene, gameTime);
			
			// execute scripts
			foreach (var actionScript in this.ObjectScripts) actionScript(scene, this, gameTime);

			// did we move?
			if (this.GetHierarchyPosition() != position)
			{
				// we have to talk to the scene about the movement. 
				scene.MoveOctree(this, position);
			} 


			foreach (GameObject obj in this.ChildObjects) obj.UpdateLogic(scene, gameTime);
		}

		public abstract void Update(GameScene scene, GameTime gameTime);

		#endregion

		public void DrawLogic(GameScene scene, SpriteBatch spriteBatch)
		{
			//Console.WriteLine($"Drawing, Position: {this.Position}");
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
					//Console.WriteLine($"POS: {this.GetHierarchyPosition().X} {this.GetHierarchyPosition().Y} {this.GetHierarchyPosition().Z}");

					var position = this.GetHierarchyPosition();
					position.Y = -position.Y;

					Matrix totalRotation = Matrix.CreateRotationX(this.Rotation.X) * Matrix.CreateRotationY(this.Rotation.Y) * Matrix.CreateRotationZ(this.Rotation.Z);

					effect.World = Matrix.CreateScale(this.GetHierarchyScale())
					               * totalRotation
					               * Matrix.CreateTranslation(position)
					               * transformMatrices[mesh.ParentBone.Index];
					effect.View = scene.GetViewMatrix();
					effect.Projection = scene.GetProjectionMatrix();

					effect.EnableDefaultLighting();
					effect.LightingEnabled = true; // Turn on the lighting subsystem.

					//effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f); // some diffuse light
					//effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
					//effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
					scene.AddLightningToEffect(effect);

					//effect.EmissiveColor = new Vector3(1, 0, 0); // Sets some strange emmissive lighting.  This just looks weird.

				}

				mesh.Draw();
			}
			
		}

	}
}
