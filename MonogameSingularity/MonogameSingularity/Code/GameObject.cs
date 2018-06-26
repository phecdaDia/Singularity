using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Collisions;
using Singularity.Events;
using Singularity.GameObjects.Interfaces;
using Singularity.Utilities;

namespace Singularity
{
	using Microsoft.SqlServer.Server;

	/// <summary>
	/// A GameObject can be any object in a GameScene.
	/// </summary>
	public abstract class GameObject
	{
		public Model
			Model { get; private set; } // Model of the entity. Is Null if the object shall not be rendered.

		public Vector3   Position  { get; private set; } // Current position of the model
		public Vector3   Rotation  { get; private set; } // Current rotation of the model
		public Vector3   Scale     { get; private set; } // Scale of the model
		public Vector3   Inertia   { get; private set; } // only used when implementing IInertia
		public Collision Collision { get; private set; }
		public Texture2D Texture { get; private set; }

		public Boolean EnablePushCollision { get; set; }

		public GameObject
			ParentObject { get; private set; } // Parent Object. This object will be in the ChildObjects of the Parent.

		public List<GameObject> ChildObjects { get; private set; } // Child Objects

		public Effect Effect { get; private set; } //Shader of Object
		public bool CullingEnabled { get; private set; } = true;
		public Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> EffectParams { get; private set; } //Params for shader
		public bool ApplySceneLight { get; private set; } = true;
		public String DebugName { get; private set; } // Used for debugging.

		public Matrix ScaleMatrix
		{
			get { return Matrix.CreateScale(this.GetHierarchyScale()); }
		}

		public Matrix RotationMatrix
		{
			get
			{
				var rotation = this.GetHierarchyRotation();

				return Matrix.CreateRotationX(rotation.X)
				       * Matrix.CreateRotationY(rotation.Y)
				       * Matrix.CreateRotationZ(rotation.Z);
			}
		}

		public Matrix TransformationMatrix
		{
			get { return ScaleMatrix * RotationMatrix; }
		}

		public float ModelRadius { get; private set; }

		private readonly List<Action<GameScene, GameObject, GameTime>> ObjectScripts; // Basic Actionscripts

		public CustomData CustomData { get; private set; }

		/// <summary>
		/// Initializing Constructor
		/// Sets default values for all properties
		/// </summary>
		protected GameObject()
		{
			// Setting default values for all members
			this.Position            = new Vector3();
			this.Rotation            = new Vector3();
			this.Scale               = Vector3.One;
			this.Inertia             = new Vector3();
			this.EnablePushCollision = true;

			this.ParentObject = null;
			this.ChildObjects = new List<GameObject>();

			this.ObjectScripts = new List<Action<GameScene, GameObject, GameTime>>();
			this.CustomData = new CustomData();
		}

		#region Builder Pattern

		#region SetModel

		/// <summary>
		/// Sets the <see cref="Model"/> for the <see cref="GameObject"/>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public GameObject SetModel(Model model)
		{
			this.Model = model;

			var center = this.GetHierarchyPosition();

			// now get max(r + r(v))
			float rm = 0.0f;
			foreach (var mesh in this.Model.Meshes)
			{
				// get distance
				var bs   = mesh.BoundingSphere;
				var dist = (bs.Center - center).Length() + bs.Radius;

				if (dist > rm) rm = dist;
			}

			this.ModelRadius = rm;

			if (this is ICollidable || this is ICollider
			) // everything that has something to do with collisions gets a sphere at the beginning
				this.SetCollision(new SphereCollision(this.ModelRadius));
			return this;
		}

		/// <summary>
		/// Sets the <see cref="Model"/> for the <see cref="GameObject"/>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public GameObject SetModel(String model)
		{
			this.SetTexture(ModelManager.GetTexture(model));

			return this.SetModel(ModelManager.GetModel(model));
		}

		#endregion

		#region SetPosition

		/// <summary>
		/// Sets the <see cref="Position"/> by calling <seealso cref="SetPosition(Vector3)"/> with the specified values.
		/// The Z part of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject SetPosition(float x, float y) => SetPosition(new Vector3(x, y, 0));

		/// <summary>
		/// Sets the <see cref="Position"/> by calling <seealso cref="SetPosition(Vector3)"/> with the specified values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetPosition(float x, float y, float z) => SetPosition(new Vector3(x, y, z));

		/// <summary>
		/// Sets the <see cref="Position"/>
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject SetPosition(Vector3 position)
		{
			this.Position = position;

			return this;
		}

		#endregion

		#region AddPosition

		/// <summary>
		/// Modifies the <see cref="Position"/> by calling <seealso cref="AddPosition(Vector3)"/> with the specified values.
		/// The Z part of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y) => AddPosition(new Vector3(x, y, 0));

		/// <summary>
		/// Modifies the <see cref="Position"/> by calling <seealso cref="AddPosition(Vector3)"/> with the specified values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y, float z) => AddPosition(new Vector3(x, y, z));

		/// <summary>
		/// Modifies the <see cref="Position"/> by adding the Vectors
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject AddPosition(Vector3 position)
		{
			this.Position += position;
			return this;
		}

		#endregion

		#region SetRotation

		/// <summary>
		/// Sets the <see cref="Rotation"/> by calling <seealso cref="SetPosition(Vector3)"/>
		/// The Z value of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject SetRotation(float x, float y) => SetRotation(new Vector3(x, y, 0));

		/// <summary>
		/// Sets the <see cref="Rotation"/> by calling <seealso cref="SetPosition(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetRotation(float x, float y, float z) => SetRotation(new Vector3(x, y, z));

		/// <summary>
		/// Sets the <see cref="Rotation"/>
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject SetRotation(Vector3 rotation)
		{
			this.Rotation = rotation;
			return this;
		}

		#endregion

		#region AddRotation

		/// <summary>
		/// Modifies the <see cref="Rotation"/> by calling <seealso cref="AddRotation(Vector3)"/>
		/// The Z value of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y) => AddRotation(new Vector3(x, y, 0));

		/// <summary>
		/// Modifies the <see cref="Rotation"/> by calling <seealso cref="AddRotation(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y, float z) => AddRotation(new Vector3(x, y, z));

		/// <summary>
		/// Modifies the <see cref="Rotation"/> by adding both <see cref="Vector3"/>
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject AddRotation(Vector3 rotation)
		{
			this.Rotation += rotation;
			return this;
		}

		#endregion

		#region SetScale

		/// <summary>
		/// Sets the <see cref="Scale"/> by calling <seealso cref="SetScale(Vector3)"/>
		/// All dimensions are set to the parameter.
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(float scale) => SetScale(scale, scale, scale);

		/// <summary>
		/// Sets the <see cref="Scale"/> by calling <seealso cref="SetScale(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetScale(float x, float y, float z) => SetScale(new Vector3(x, y, z));

		/// <summary>
		/// Sets the <see cref="Scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(Vector3 scale)
		{
			this.Scale = scale;
			return this;
		}

		#endregion

		#region MultiplyScale

		/// <summary>
		/// Multiplies <see cref="Scale"/> by calling <seealso cref="MultiplyScale(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(float x, float y, float z) => MultiplyScale(new Vector3(x, y, z));

		/// <summary>
		/// Multiplies <see cref="Scale"/> with <paramref name="scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(Vector3 scale)
		{
			this.Scale *= scale;
			return this;
		}

		#endregion

		#region AddScale

		/// <summary>
		/// Modifies <see cref="Scale"/> by calling <seealso cref="AddScale(Vector3)"/>
		/// All dimension of the <see cref="Vector3"/> will be set to <paramref name="scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(float scale) => AddScale(new Vector3(scale));

		/// <summary>
		/// Modifies <see cref="Scale"/> by calling <seealso cref="AddScale(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddScale(float x, float y, float z) => AddScale(new Vector3(x, y, z));

		/// <summary>
		/// Adds <paramref name="scale"/> to the <see cref="Scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(Vector3 scale)
		{
			this.Scale += scale;
			return this;
		}

		#endregion

		#region SetParent

		/// <summary>
		/// Sets the <see cref="ParentObject"/>
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public GameObject SetParent(GameObject parent)
		{
			this.ParentObject = parent;
			parent.ChildObjects.Add(this);

			return this;
		}

		#endregion

		#region AddScript

		/// <summary>
		/// Adds a <see cref="Action"/> to the Scripts, which will be executes after <seealso cref="Update"/> is called.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public GameObject AddScript(Action<GameScene, GameObject, GameTime> script)
		{
			this.ObjectScripts.Add(script);

			return this;
		}

		#endregion

		#region AddCollisionEvent

		public GameObject AddCollisionEvent(Action<GameObject, GameScene, Vector3, Vector3> collEvent)
		{
			Debug.Assert(collEvent != null, nameof(collEvent) + " != null");

			this.OnCollisionEvent += (s, e) => collEvent(e.Collidable, e.Scene, e.Position, e.Normal);
			return this;
		}

		#endregion

		#region AddChild

		/// <summary>
		/// Adds a Childobject which will move relative to this <see cref="GameObject"/>
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public GameObject AddChild(GameObject child)
		{
			this.ChildObjects.Add(child);
			child.ParentObject = this;
			return this;
		}

		#endregion

		#region SetDebugName

		/// <summary>
		/// Sets the <see cref="DebugName"/> for testing.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject SetDebugName(String name)
		{
			this.DebugName = name;
			return this;
		}

		#endregion

		#region SetCollision

		public GameObject SetCollision(Collision collision)
		{
			this.Collision = (Collision) collision.Clone();
			this.Collision.SetParent(this);
			return this;
		}

		#endregion

		#region SetInertia

		public GameObject SetInertia(float x, float y) => SetInertia(x, y, 0);

		public GameObject SetInertia(float x, float y, float z) => SetInertia(new Vector3(x, y, z));

		public GameObject SetInertia(Vector3 inertia)
		{
			if (!(this is IInertia))
			{
				// give out a warning, that inertia should not be used
				Console.WriteLine($"Not inheriting IInertia. Inertia should not be used!");
			}


			this.Inertia = inertia;
			return this;
		}

		#endregion

		#region AddInertia

		public GameObject AddInertia(float x, float y) => AddInertia(x, y, 0);

		public GameObject AddInertia(float x, float y, float z) => AddInertia(new Vector3(x, y, z));

		public GameObject AddInertia(Vector3 inertia)
		{
			if (!(this is IInertia))
			{
				// give out a warning, that inertia should not be used
				Console.WriteLine($"Not inheriting IInertia. Inertia should not be used!");
			}

			this.Inertia += inertia;
			return this;
		}

		#endregion

		#region SetEnableCollision

		public GameObject SetEnableCollision(Boolean enable)
		{
			this.EnablePushCollision = enable;

			return this;
		}

		#endregion

		#region SetEffect

		public GameObject SetEffect(Effect effect, Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams)
		{
			this.Effect = effect;
			this.EffectParams = effectParams;
			return this;
		}

		#endregion

		#region SetEffect

		public GameObject SetTexture(Texture2D texture)
		{
			this.Texture = texture;
			return this;
		}

		#endregion

		#region SetSceneLight

		public GameObject SetSceneLight(bool set)
		{
			this.ApplySceneLight = set;
			return this;
		}

		#endregion

		#region SetCulling

		public GameObject SetCulling(bool enabled)
		{
			this.CullingEnabled = enabled;
			return this;
		}

		#endregion

		#endregion

		/// <summary>
		/// Return the multiplies <see cref="Scale"/> from this <see cref="GameObject"/> and the <see cref="ParentObject"/> <seealso cref="GetHierarchyScale()"/>
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyScale()
		{
			if (this.ParentObject == null) return this.Scale;
			return this.Scale * this.ParentObject.GetHierarchyScale();
		}

		/// <summary>
		/// Return the added <see cref="Position"/> from this <see cref="GameObject"/> and the <see cref="ParentObject"/> <seealso cref="GetHierarchyPosition()"/>
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyPosition()
		{
			if (this.ParentObject == null) return this.Position;
			return Vector3.Transform(this.Position, this.ParentObject.RotationMatrix) +
			       this.ParentObject.GetHierarchyPosition();
		}


		public Vector3 GetHierarchyRotation()
		{
			if (this.ParentObject == null) return this.Rotation;
			return this.Rotation + this.ParentObject.GetHierarchyRotation();
		}


		/// <summary>
		/// Gets all <see cref="Action"/>scripts set to this <see cref="GameObject"/>
		/// </summary>
		/// <returns></returns>
		public List<Action<GameScene, GameObject, GameTime>> GetScripts()
		{
			return this.ObjectScripts;
		}

		// By https://pastebin.com/47vwJWSc

		/// <summary>
		/// Calculate <seealso cref="BoundingBox"/> for this 
		/// </summary>
		/// <returns></returns>
		public BoundingBox GetBoundingBox()
		{
			return GetBoundingBox(
			                      this.Model,
			                      Matrix.CreateRotationX(this.Rotation.X) * Matrix.CreateRotationY(this.Rotation.Y) *
			                      Matrix.CreateRotationZ(this.Rotation.Z)
			                      * Matrix.CreateScale(this.GetHierarchyScale())
			                     );
		}

		/// <summary>
		/// Calculate <seealso cref="BoundingBox"/> for <seealso cref="Microsoft.Xna.Framework.Graphics.Model"/>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="worldTransformation"></param>
		/// <returns></returns>
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
					int vertexStride     = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
					int vertexBufferSize = meshPart.NumVertices * vertexStride;

					// Get vertex data as float
					float[] vertexData = new float[vertexBufferSize / sizeof(float)];
					meshPart.VertexBuffer.GetData<float>(vertexData);

					// Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
					for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
					{
						Vector3 transformedPosition =
							Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]),
							                  worldTransformation);

						min = Vector3.Min(min, transformedPosition);
						max = Vector3.Max(max, transformedPosition);
					}
				}
			}

			// Create and return bounding box
			return new BoundingBox(min, max);
		}

		#region Abstract Methods

		/// <summary>
		/// Calls <seealso cref="Update"/>, and calls back to the scene. 
		/// After that all <see cref="ChildObjects"/> will be updated.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public void UpdateLogic(GameScene scene, GameTime gameTime)
		{
			// get a copy of the position
			var position = this.GetHierarchyPosition();

			Update(scene, gameTime);

			// add inertia.
			if (this is IInertia)
				this.Position += this.Inertia * (float) gameTime.ElapsedGameTime.TotalSeconds;

			// execute scripts
			foreach (var actionScript in this.ObjectScripts) actionScript(scene, this, gameTime);


			// check if we are even able to stay here.
			scene.HandleCollision(this, position);

			// did we move?
			if (this.GetHierarchyPosition() != position)
			{
				// we have to talk to the scene about the movement. 
				scene.MoveOctree(this, position);
			}

			// if we are allowed to move the camera, do it

			scene.CameraLocked = false;
			if (this is ICameraController controller) controller.SetCamera(scene);

			scene.CameraLocked = true;

			foreach (GameObject obj in this.ChildObjects) obj.UpdateLogic(scene, gameTime);
		}

		/// <summary>
		/// Updates the <see cref="GameObject"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public abstract void Update(GameScene scene, GameTime gameTime);

		/// <summary>
		/// Calls <seealso cref="Draw"/>
		/// After that draws all <see cref="ChildObjects"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="spriteBatch"></param>
		public void DrawLogic(GameScene scene, SpriteBatch spriteBatch, GameObjectDrawMode drawMode = GameObjectDrawMode.All)
		{
			DrawLogic(scene, spriteBatch, scene.GetViewMatrix(), scene.GetProjectionMatrix(), drawMode);
		}

		public void DrawLogic(GameScene scene, SpriteBatch spriteBatch, Matrix view, Matrix projection,
			GameObjectDrawMode drawMode = GameObjectDrawMode.All)
		{

			//Console.WriteLine($"Drawing, Position: {this.Position}");
			if ((drawMode & GameObjectDrawMode.Model) > 0) Draw(scene, view, projection);
			if ((drawMode & GameObjectDrawMode.SpriteBatch) > 0) Draw2D(spriteBatch);

			foreach (GameObject obj in this.ChildObjects) obj.DrawLogic(scene, spriteBatch, view, projection, drawMode);
		}

		public void DrawLogicWithEffect(
			GameScene scene,
			SpriteBatch spriteBatch,
			Effect effect,
			Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams,
			String technique = null,
			GameObjectDrawMode drawMode = GameObjectDrawMode.All)
		{
			
			if ((drawMode & GameObjectDrawMode.Model) > 0) DrawWithSpecificEffect(scene, effect, effectParams, technique);
			if ((drawMode & GameObjectDrawMode.SpriteBatch) > 0) Draw2D(spriteBatch);

			foreach (GameObject obj in this.ChildObjects) obj.DrawLogicWithEffect(scene, spriteBatch, effect, effectParams, technique, drawMode);
		}

		protected virtual void Draw2D(SpriteBatch spriteBatch)
		{}

		/// <summary>
		/// Checks if there is a <see cref="Model"/> to draw and draws it.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="spriteBatch"></param>
		protected virtual void Draw(GameScene scene, Matrix view, Matrix projection)
		{
			if (this.Model == null) return; // No model means it can't be rendered.

			// copy the scale of bones from the model to apply it later.
			var transformMatrices = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(transformMatrices);

			var originalRastState = scene.Game.GraphicsDevice.RasterizerState;

			if (!CullingEnabled)
			{
				var newRastState = new RasterizerState
				{
					CullMode             = CullMode.None,
					DepthBias            = originalRastState.DepthBias,
					DepthClipEnable      = originalRastState.DepthClipEnable,
					FillMode             = originalRastState.FillMode,
					MultiSampleAntiAlias = originalRastState.MultiSampleAntiAlias,
					ScissorTestEnable    = originalRastState.ScissorTestEnable,
					SlopeScaleDepthBias  = originalRastState.SlopeScaleDepthBias
				};

				scene.Game.GraphicsDevice.RasterizerState = newRastState;
			}

			foreach (ModelMesh mesh in this.Model.Meshes)
			{
				if (this.Effect == null)
					foreach (Effect effect in mesh.Effects)
					{
						if (!(effect is BasicEffect))
							continue;

						BasicEffect basisEffect = (BasicEffect)effect;

						// calculating the full rotation of our object.
						//Console.WriteLine($"POS: {this.GetHierarchyPosition().X} {this.GetHierarchyPosition().Y} {this.GetHierarchyPosition().Z}");

						basisEffect.World = transformMatrices[mesh.ParentBone.Index]
						               * this.ScaleMatrix
						               * this.RotationMatrix
						               * Matrix.CreateTranslation(this.GetHierarchyPosition());

						basisEffect.View = view;
						basisEffect.Projection = projection;

						scene.AddLightningToEffect(basisEffect);

					}
				else
					foreach (var part in mesh.MeshParts)
					{
						part.Effect = this.Effect;

						this.EffectParams.Invoke(this, part.Effect, transformMatrices, mesh, scene);
						if(ApplySceneLight) scene.AddLightningToEffect(part.Effect);
					}

				mesh.Draw();
			}

			if (!CullingEnabled)
				scene.Game.GraphicsDevice.RasterizerState = originalRastState;
		}

		/// <summary>
		/// Checks if there is a <see cref="Model"/> to draw and draws it with specified Effect.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="spriteBatch"></param>
		protected virtual void DrawWithSpecificEffect(GameScene scene, Effect effect, Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams, string technique = null)
		{
			if (this.Model == null) return; // No model means it can't be rendered.

			// copy the scale of bones from the model to apply it later.
			var transformMatrices = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(transformMatrices);

			var originalRastState = scene.Game.GraphicsDevice.RasterizerState;

			if (!CullingEnabled)
			{
				var newRastState = new RasterizerState
				{
					CullMode = CullMode.None,
					DepthBias = originalRastState.DepthBias,
					DepthClipEnable = originalRastState.DepthClipEnable,
					FillMode = originalRastState.FillMode,
					MultiSampleAntiAlias = originalRastState.MultiSampleAntiAlias,
					ScissorTestEnable = originalRastState.ScissorTestEnable,
					SlopeScaleDepthBias = originalRastState.SlopeScaleDepthBias
				};

				scene.Game.GraphicsDevice.RasterizerState = newRastState;
			}

			if (technique == null)
			{
				foreach (var pass in effect.CurrentTechnique.Passes)
				{
					foreach (var mesh in Model.Meshes)
					{
						foreach (var part in mesh.MeshParts)
						{
							part.Effect = effect;
							effectParams.Invoke(this, effect, transformMatrices, mesh, scene);
							//if (applySceneLighting) scene.AddLightningToEffect(part.Effect);
						}
						mesh.Draw();
					}
				}
			}
			else
			{
				effect.CurrentTechnique = effect.Techniques[technique];

				foreach (var pass in effect.CurrentTechnique.Passes)
				{
					foreach (var mesh in Model.Meshes)
					{
						foreach (var part in mesh.MeshParts)
						{
							part.Effect = effect;
							effectParams.Invoke(this, effect, transformMatrices, mesh, scene);
							//if (applySceneLighting) scene.AddLightningToEffect(part.Effect);
						}
						mesh.Draw();
					}
				}
			}
				

			if (!CullingEnabled)
				scene.Game.GraphicsDevice.RasterizerState = originalRastState;
		}

		public virtual void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
		}

		public virtual void UnloadContent()
		{
		}

		#endregion

		#region Events

		protected event EventHandler<CollisionEventArgs> OnCollisionEvent;

		public virtual void OnCollision(GameObject collider, GameObject collidable, GameScene scene, Vector3 position,
		                                Vector3    normal) =>
			OnCollision(new CollisionEventArgs(position, normal, collider, collidable, scene));

		public virtual void OnCollision(CollisionEventArgs e) =>
			OnCollisionEvent?.Invoke(this, e);

		#endregion
	}

	public enum GameObjectDrawMode
	{
		Nothing     = 0b00000000,	// Object won't be drawn
		Model       = 0b00000001,	// Only the 3d part will be drawn
		SpriteBatch = 0b00000010,	// Only 2d will be drawn
		All         = 0b11111111	// Everything will be drawn
		
	}
}