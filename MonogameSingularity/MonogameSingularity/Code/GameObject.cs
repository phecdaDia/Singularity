using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Core.Collisions;
using Singularity.Core.Events;
using Singularity.Core.GameObjects;
using Singularity.Core.Utilities;

namespace Singularity.Core
{
	/// <summary>
	///     A GameObject can be any object in a GameScene.
	/// </summary>
	public abstract class GameObject
	{
		public List<GameObject> ChildrenBuffer { get; } = new List<GameObject>();

		private readonly List<Action<GameScene, GameObject, GameTime>> ObjectScripts = new List<Action<GameScene, GameObject, GameTime>>();// Basic Actionscripts

		/// <summary>
		///     Initializing Constructor
		///     Sets default values for all properties
		/// </summary>
		protected GameObject()
		{}

		public String ModelPath { get; private set; }

		public Model Model
		{
			get { return ModelManager.GetModel(this.ModelPath); }
		}

		public Vector3 Position { get; private set; } // Current position of the model
		public Vector3 Rotation { get; private set; } // Current rotation of the model
		public Vector3 Scale { get; private set; } = Vector3.One;// Scale of the model
		public Vector3 Inertia { get; private set; } // only used when implementing IInertia
		public Collision Collision { get; private set; }

		public Texture2D Texture
		{
			get { return ModelManager.GetTexture(this.ModelPath); }
		}

		public bool EnablePushCollision { get; private set; } = true;

		public GameObject ParentObject { get; private set; } // Parent Object. This object will be in the ChildObjects of the Parent.

		public List<GameObject> ChildObjects { get; } = new List<GameObject>();// Child Objects

		public Effect Effect { get; private set; } //Shader of Object
		public bool CullingEnabled { get; private set; } = true;

		public Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> EffectParams { get; private set; } //Params for shader

		public bool ApplySceneLight { get; private set; } = true;
		public string DebugName { get; private set; } // Used for debugging.

		public ChildProperties ChildProperties { get; private set; } = ChildProperties.All;

		public GameObjectDrawMode DrawMode { get; private set; } = GameObjectDrawMode.All;

		public RotationMode RotationMode { get; private set; } = RotationMode.XYZ;

		public Matrix WorldMatrix
		{
			get
			{
				return this.ScaleMatrix *
				       this.RotationMatrix *
				       this.TranslationMatrix;
			}
		}

		public Matrix TranslationMatrix
		{
			get { return Matrix.CreateTranslation(this.GetHierarchyPosition()); }
		}

		public Matrix ScaleMatrix
		{
			get { return Matrix.CreateScale(this.GetHierarchyScale()); }
		}

		public Matrix RotationMatrix
		{
			get
			{
				var rotation = this.GetHierarchyRotation();

				//Matrix.CreateRotationX(rotation.Z)
				//	* Matrix.CreateRotationY(rotation.Y)
				//	* Matrix.CreateRotationZ(rotation.X);

				Matrix m = Matrix.Identity;
				for (int counter = 0; counter < 8; counter += 2)
				{
					var rmp = ((int) this.RotationMode >> counter) & 0b11;
					if (rmp == (int)RotationMode.X) m *= Matrix.CreateRotationX(rotation.X);
					else if (rmp == (int)RotationMode.Y) m *= Matrix.CreateRotationY(rotation.Y);
					else if (rmp == (int)RotationMode.Z) m *= Matrix.CreateRotationZ(rotation.Z);
				}

				return m;
			}
		}

		public Matrix TransformationMatrix
		{
			get { return this.ScaleMatrix * this.RotationMatrix; }
		}

		public float ModelRadius { get; private set; }

		public CustomData CustomData { get; } = new CustomData();

		/// <summary>
		///     Return the multiplies <see cref="Scale" /> from this <see cref="GameObject" /> and the <see cref="ParentObject" />
		///     <seealso cref="GetHierarchyScale()" />
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyScale()
		{
			if (this.ParentObject == null || !this.ChildProperties.HasFlag(ChildProperties.Scale)) return this.Scale;
			return this.Scale * this.ParentObject.GetHierarchyScale();
		}

		/// <summary>
		///     Return the added <see cref="Position" /> from this <see cref="GameObject" /> and the <see cref="ParentObject" />
		///     <seealso cref="GetHierarchyPosition()" />
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyPosition()
		{
			if (this.ParentObject == null) return this.Position;
			if (this.ChildProperties.HasFlag(ChildProperties.TranslationRotation))
				return Vector3.Transform(this.Position, this.ParentObject.RotationMatrix) + this.ParentObject.GetHierarchyPosition();
			if (this.ChildProperties.HasFlag(ChildProperties.Translation)) return this.Position + this.ParentObject.GetHierarchyPosition();

			return this.Position;
		}


		public Vector3 GetHierarchyRotation()
		{
			if (this.ParentObject == null || !this.ChildProperties.HasFlag(ChildProperties.Rotation)) return this.Rotation;
			return this.Rotation + this.ParentObject.GetHierarchyRotation();
		}

		public GameObjectDrawMode GetHierarchyDrawMode()
		{
			if (this.ParentObject == null || !this.ChildProperties.HasFlag(ChildProperties.DrawMode)) return this.DrawMode;
			return this.ParentObject.GetHierarchyDrawMode();
		}


		/// <summary>
		///     Gets all <see cref="Action" />scripts set to this <see cref="GameObject" />
		/// </summary>
		/// <returns></returns>
		public List<Action<GameScene, GameObject, GameTime>> GetScripts()
		{
			return this.ObjectScripts;
		}

		#region Builder Pattern

		#region SetModel

		/// <summary>
		///     Sets the <see cref="Model" /> for the <see cref="GameObject" />
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public GameObject SetModel(string model)
		{
			//SetTexture(ModelManager.GetTexture(model));

			this.ModelPath = model;

			var center = this.GetHierarchyPosition();

			// now get max(r + r(v))
			var rm = 0.0f;
			foreach (var mesh in this.Model.Meshes)
			{
				// get distance
				var bs = mesh.BoundingSphere;
				var dist = (bs.Center - center).Length() + bs.Radius;

				if (dist > rm) rm = dist;
			}

			this.ModelRadius = rm;

			//if (this is ICollidable || this is ICollider) // everything that has something to do with collisions gets a sphere at the beginning
			//	SetCollision(new SphereCollision(ModelRadius));
			return this;
		}

		#endregion

		#region SetPosition

		/// <summary>
		///     Sets the <see cref="Position" /> by calling <seealso cref="SetPosition(Vector3)" /> with the specified values.
		///     The Z part of the <see cref="Vector3" /> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject SetPosition(float x, float y)
		{
			return this.SetPosition(new Vector3(x, y, 0));
		}

		/// <summary>
		///     Sets the <see cref="Position" /> by calling <seealso cref="SetPosition(Vector3)" /> with the specified values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetPosition(float x, float y, float z)
		{
			return this.SetPosition(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets the <see cref="Position" />
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject SetPosition(Vector3 position)
		{
			if (this.ParentObject != null && this.ChildProperties.HasFlag(ChildProperties.KeepPositon))
			{
				var mat = Matrix.Identity;

				if (this.ChildProperties.HasFlag(ChildProperties.Rotation)) mat *= this.ParentObject.RotationMatrix;
				if (this.ChildProperties.HasFlag(ChildProperties.Translation)) mat *= this.ParentObject.TranslationMatrix;

				this.Position = Vector3.Transform(position, Matrix.Invert(mat));
			}
			else
			{
				this.Position = position;
			}

			return this;
		}

		public GameObject SetPositionAt(Axis axis, float value)
		{

			if (axis.HasFlag(Axis.X)) this.SetPosition(value, this.Position.Y, this.Position.Z);
			if (axis.HasFlag(Axis.Y)) this.SetPosition(this.Position.X, value, this.Position.Z);
			if (axis.HasFlag(Axis.Z)) this.SetPosition(this.Position.X, this.Position.Y, value);

			return this;
		}

		#endregion

		#region AddPosition

		/// <summary>
		///     Modifies the <see cref="Position" /> by calling <seealso cref="AddPosition(Vector3)" /> with the specified values.
		///     The Z part of the <see cref="Vector3" /> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y)
		{
			return this.AddPosition(new Vector3(x, y, 0));
		}

		/// <summary>
		///     Modifies the <see cref="Position" /> by calling <seealso cref="AddPosition(Vector3)" /> with the specified values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y, float z)
		{
			return this.AddPosition(new Vector3(x, y, z));
		}

		/// <summary>
		///     Modifies the <see cref="Position" /> by adding the Vectors
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject AddPosition(Vector3 position)
		{
			this.Position += position;
			return this;
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y, GameTime gameTime)
		{
			return this.AddPosition(new Vector3(x, y, 0), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y, float z, GameTime gameTime)
		{
			return this.AddPosition(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     Modifies the <see cref="Position" /> by adding the Vectors multiplied by the deltaTime
		/// </summary>
		/// <param name="position"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddPosition(Vector3 position, GameTime gameTime)
		{

			return this.AddPosition(position * (float) Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddPositionAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.AddPosition(value, 0, 0);
			if (axis.HasFlag(Axis.Y)) this.AddPosition(0, value, 0);
			if (axis.HasFlag(Axis.Z)) this.AddPosition(0, 0, value);

			return this;
		}

		public GameObject AddPositionAt(Axis axis, float value, GameTime gameTime) =>
			this.AddPositionAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

		#endregion

		#region SetRotation

		/// <summary>
		///     Sets the <see cref="Rotation" /> by calling <seealso cref="SetPosition(Vector3)" />
		///     The Z value of the <see cref="Vector3" /> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject SetRotation(float x, float y)
		{
			return this.SetRotation(new Vector3(x, y, 0));
		}

		/// <summary>
		///     Sets the <see cref="Rotation" /> by calling <seealso cref="SetPosition(Vector3)" />
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetRotation(float x, float y, float z)
		{
			return this.SetRotation(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets the <see cref="Rotation" />
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject SetRotation(Vector3 rotation)
		{
			this.Rotation = rotation;
			return this;
		}
		public GameObject SetRotationAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.SetRotation(value, this.Rotation.Y, this.Rotation.Z);
			if (axis.HasFlag(Axis.Y)) this.SetRotation(this.Rotation.X, value, this.Rotation.Z);
			if (axis.HasFlag(Axis.Z)) this.SetRotation(this.Rotation.X, this.Rotation.Y, value);

			return this;
		}

		#endregion

		#region AddRotation

		/// <summary>
		///     Modifies the <see cref="Rotation" /> by calling <seealso cref="AddRotation(Vector3)" />
		///     The Z value of the <see cref="Vector3" /> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y)
		{
			return this.AddRotation(new Vector3(x, y, 0));
		}

		/// <summary>
		///     Modifies the <see cref="Rotation" /> by calling <seealso cref="AddRotation(Vector3)" />
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y, float z)
		{
			return this.AddRotation(new Vector3(x, y, z));
		}

		/// <summary>
		///     Modifies the <see cref="Rotation" /> by adding both <see cref="Vector3" />
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject AddRotation(Vector3 rotation)
		{
			this.Rotation += rotation;
			return this;
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y, GameTime gameTime)
		{
			return this.AddRotation(new Vector3(x, y, 0), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y, float z, GameTime gameTime)
		{
			return this.AddRotation(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="rotation"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddRotation(Vector3 rotation, GameTime gameTime)
		{
			return this.AddRotation(rotation * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddRotationAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.AddRotation(value, 0, 0);
			if (axis.HasFlag(Axis.Y)) this.AddRotation(0, value, 0);
			if (axis.HasFlag(Axis.Z)) this.AddRotation(0, 0, value);

			return this;
		}
		public GameObject AddRotationAt(Axis axis, float value, GameTime gameTime) =>
			this.AddRotationAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

		#endregion

		#region SetScale

		/// <summary>
		///     Sets the <see cref="Scale" /> by calling <seealso cref="SetScale(Vector3)" />
		///     All dimensions are set to the parameter.
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(float scale)
		{
			return this.SetScale(scale, scale, scale);
		}

		/// <summary>
		///     Sets the <see cref="Scale" /> by calling <seealso cref="SetScale(Vector3)" />
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetScale(float x, float y, float z)
		{
			return this.SetScale(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets the <see cref="Scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(Vector3 scale)
		{
			this.Scale = scale;
			return this;
		}
		public GameObject SetScaleAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.SetScale(value, this.Scale.Y, this.Scale.Z);
			if (axis.HasFlag(Axis.Y)) this.SetScale(this.Scale.X, value, this.Scale.Z);
			if (axis.HasFlag(Axis.Z)) this.SetScale(this.Scale.X, this.Scale.Y, value);

			return this;
		}

		#endregion

		#region MultiplyScale

		/// <summary>
		///     Multiplies <see cref="Scale" /> by calling <seealso cref="MultiplyScale(Vector3)" />
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(float x, float y, float z)
		{
			return this.MultiplyScale(new Vector3(x, y, z));
		}

		/// <summary>
		///     Multiplies <see cref="Scale" /> with <paramref name="scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(Vector3 scale)
		{
			this.Scale *= scale;
			return this;
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(float x, float y, float z, GameTime gameTime)
		{
			return this.MultiplyScale(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(Vector3 scale, GameTime gameTime)
		{
			var s = new Vector3();
			var framerate = Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate);
			s.X = (float) Math.Pow(scale.X, framerate);
			s.Y = (float) Math.Pow(scale.Y, framerate);
			s.Z = (float) Math.Pow(scale.Z, framerate);
			
			return this.MultiplyScale(s);
		}
		public GameObject MultiplyScaleAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.MultiplyScale(value, 1, 1);
			if (axis.HasFlag(Axis.Y)) this.MultiplyScale(1, value, 1);
			if (axis.HasFlag(Axis.Z)) this.MultiplyScale(1, 1, value);

			return this;
		}
		public GameObject MultiplyScaleAt(Axis axis, float value, GameTime gameTime) =>
			this.MultiplyScaleAt(axis, (float) Math.Pow(value, Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate)));

		#endregion

		#region AddScale

		/// <summary>
		///     Modifies <see cref="Scale" /> by calling <seealso cref="AddScale(Vector3)" />
		///     All dimension of the <see cref="Vector3" /> will be set to <paramref name="scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(float scale)
		{
			return this.AddScale(new Vector3(scale));
		}

		/// <summary>
		///     Modifies <see cref="Scale" /> by calling <seealso cref="AddScale(Vector3)" />
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddScale(float x, float y, float z)
		{
			return this.AddScale(new Vector3(x, y, z));
		}

		/// <summary>
		///     Adds <paramref name="scale" /> to the <see cref="Scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(Vector3 scale)
		{
			this.Scale += scale;
			return this;
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddScale(float scale, GameTime gameTime)
		{
			return this.AddScale(new Vector3(scale), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddScale(float x, float y, float z, GameTime gameTime)
		{
			return this.AddScale(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddScale(Vector3 scale, GameTime gameTime)
		{
			return this.AddScale(scale * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddScaleAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.AddScale(value, 0, 0);
			if (axis.HasFlag(Axis.Y)) this.AddScale(0, value, 0);
			if (axis.HasFlag(Axis.Z)) this.AddScale(0, 0, value);

			return this;
		}
		public GameObject AddScaleAt(Axis axis, float value, GameTime gameTime) =>
			this.AddScaleAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

		#endregion

		#region SetInertia

		public GameObject SetInertia(float x, float y)
		{
			return this.SetInertia(x, y, 0);
		}

		public GameObject SetInertia(float x, float y, float z)
		{
			return this.SetInertia(new Vector3(x, y, z));
		}

		public GameObject SetInertia(Vector3 inertia)
		{
			if (!(this is IInertia)) Console.WriteLine($"Not inheriting IInertia. Inertia should not be used!");


			this.Inertia = inertia;
			return this;
		}
		public GameObject SetInertiaAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) this.SetInertia(value, this.Inertia.Y, this.Inertia.Z);
			if (axis.HasFlag(Axis.Y)) this.SetInertia(this.Inertia.X, value, this.Inertia.Z);
			if (axis.HasFlag(Axis.Z)) this.SetInertia(this.Inertia.X, this.Inertia.Y, value);

			return this;
		}

		#endregion

		#region AddInertia

		public GameObject AddInertia(float x, float y)
		{
			return this.AddInertia(x, y, 0);
		}

		public GameObject AddInertia(float x, float y, float z)
		{
			return this.AddInertia(new Vector3(x, y, z));
		}

		public GameObject AddInertia(Vector3 inertia)
		{
			if (!(this is IInertia)) Console.WriteLine($"Not inheriting IInertia. Inertia should not be used!");

			this.Inertia += inertia;
			return this;
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddInertia(float x, float y, GameTime gameTime)
		{
			return this.AddInertia(new Vector3(x, y, 0), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddInertia(float x, float y, float z, GameTime gameTime)
		{
			return this.AddInertia(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="inertia"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddInertia(Vector3 inertia, GameTime gameTime)
		{
			return this.AddInertia(inertia * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddInertiaAt(Axis axis, float value)
		{
			switch (axis)
			{
				case Axis.X:
					return this.AddInertia(value, 0, 0);
				case Axis.Y:
					return this.AddInertia(0, value, 0);
				case Axis.Z:
					return this.AddInertia(0, 0, value);
				case Axis.W:
					return this; // doesn't use W axis.
				default:
					throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
			}
		}
		public GameObject AddInertiaAt(Axis axis, float value, GameTime gameTime) =>
			this.AddInertiaAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

		#endregion

		#region SetParent

		/// <summary>
		///     Sets the <see cref="ParentObject" />
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public GameObject SetParent(GameObject parent, ChildProperties properties = ChildProperties.All)
		{
			parent.AddChild(this, properties);

			return this;
		}

		#endregion

		#region RemoveParent

		/// <summary>
		///     Removes the <see cref="ParentObject" />
		/// </summary>
		/// <returns></returns>
		public GameObject RemoveParent()
		{
			if (this.ParentObject == null) return this;

			this.ParentObject.RemoveChild(this);

			return this;
		}

		#endregion

		#region AddScript

		/// <summary>
		///     Adds a <see cref="Action" /> to the Scripts, which will be executes after <seealso cref="Update" /> is called.
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

		public GameObject AddCollisionEvent(Action<CollisionEventArgs> collEvent)
		{
			Debug.Assert(collEvent != null);

			this.CollisionEvent += (s, e) => collEvent(e);
			return this;
		}

		#endregion

		#region AddChild

		/// <summary>
		///     Adds a Childobject which will move relative to this <see cref="GameObject" />
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public GameObject AddChild(GameObject child, ChildProperties properties = ChildProperties.All)
		{
			child.SetChildProperties(properties);
			if (this.ChildrenBuffer.Contains(child) || this.ChildObjects.Contains(child))
				return this;

			this.ChildrenBuffer.Add(child);

			child.ParentObject = this;

			if (properties.HasFlag(ChildProperties.KeepPositon)) child.SetPosition(child.Position);

			return this;
		}

		#endregion

		#region RemoveChild

		/// <summary>
		///     Removes a Childobject
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public GameObject RemoveChild(GameObject child)
		{
			if (this.ChildObjects.Contains(child))
			{
				this.ChildObjects.Remove(child);
				child.ParentObject = null;

				if (child.ChildProperties.HasFlag(ChildProperties.KeepPositon))
				{
					var mat = Matrix.Identity;

					if (child.ChildProperties.HasFlag(ChildProperties.Rotation)) mat *= this.RotationMatrix;
					if (child.ChildProperties.HasFlag(ChildProperties.Translation)) mat *= this.TranslationMatrix;


					child.SetPosition(Vector3.Transform(child.Position, mat));
				}

				child.SetChildProperties(ChildProperties.All);
			}

			return this;
		}

		#endregion

		#region SetChildProperties

		public GameObject SetChildProperties(ChildProperties properties)
		{
			this.ChildProperties = properties;

			return this;
		}

		#endregion

		#region SetDebugName

		/// <summary>
		///     Sets the <see cref="DebugName" /> for testing.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject SetDebugName(string name)
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

		#region SetEnableCollision

		public GameObject SetEnableCollision(bool enable)
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

		#region SetTexture

		//public GameObject SetTexture(Texture2D texture)
		//{
		//	Texture = texture;
		//	return this;
		//}

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

		#region SetDrawMode

		public GameObject SetDrawMode(GameObjectDrawMode drawMode)
		{
			this.DrawMode = drawMode;
			return this;
		}

		#endregion

		#region SetRotationMode

		public GameObject SetRotationMode(RotationMode rotationMode)
		{
			this.RotationMode = rotationMode;

			return this;
		}

		#endregion

		#endregion

		#region Methods

		public void DecayInertia(Axis axis, GameTime gameTime, float factor, float minimum = 0.01f)
		{
			if (!(this is IInertia)) throw new Exception("This object does not implement inertia!");

			if (factor < 0.0f) throw new ArgumentException("decay factor is negative");
			if (factor >= 1.0f) throw new ArgumentException("decay factor is larger than 1");

			var framerate = Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate);

			var reduceFactor = (float) Math.Pow(factor, framerate);

			if (axis.HasFlag(Axis.X))
			{
				var value = this.Inertia.X * reduceFactor;
				this.SetInertiaAt(Axis.X, Math.Abs(value) < minimum ? 0f : value);
			}

			if (axis.HasFlag(Axis.Y))
			{
				var value = this.Inertia.Y * reduceFactor;
				this.SetInertiaAt(Axis.Y, Math.Abs(value) < minimum ? 0f : value);
			}

			if (axis.HasFlag(Axis.Z))
			{
				var value = this.Inertia.Z * reduceFactor;
				this.SetInertiaAt(Axis.Z, Math.Abs(value) < minimum ? 0f : value);
			}

		}

		#endregion

		#region Logic Methods

		/// <summary>
		///     Calls <seealso cref="Update" />, and calls back to the scene.
		///     After that all <see cref="ChildObjects" /> will be updated.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public void UpdateLogic(GameScene scene, GameTime gameTime)
		{
			// get a copy of the position
			var position = this.GetHierarchyPosition();

			var cbArray = this.ChildrenBuffer.ToArray();
			this.ChildrenBuffer.Clear();

			this.Update(scene, gameTime);

			this.ChildObjects.AddRange(this.ChildrenBuffer);
			//scene.AddObject(this.ChildrenBuffer);
			this.ChildrenBuffer.Clear();

			// add inertia.
			if (this is IInertia)
				this.AddPosition(this.Inertia, gameTime);

			// execute scripts
			foreach (var actionScript in this.ObjectScripts) actionScript(scene, this, gameTime);


			// check if we are even able to stay here.
			scene.HandleCollision(gameTime, this, this.GetHierarchyPosition());

			// did we move?
			if (this.GetHierarchyPosition() != position) scene.MoveOctree(this, position);

			// if we are allowed to move the camera, do it

			scene.CameraLocked = false;
			if (this is ICameraController controller) controller.SetCamera(scene);

			scene.CameraLocked = true;

			foreach (var obj in this.ChildObjects.ToArray())
				obj.UpdateLogic(scene, gameTime);

			this.ChildObjects.AddRange(cbArray);
		}

		/// <summary>
		///     Updates the <see cref="GameObject" />
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public abstract void Update(GameScene scene, GameTime gameTime);

		/// <summary>
		///     Calls <seealso cref="Draw" />
		///     After that draws all <see cref="ChildObjects" />
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="drawMode"></param>
		public void DrawLogic(GameScene scene, SpriteBatch spriteBatch, GameObjectDrawMode drawMode = GameObjectDrawMode.All)
		{
			if (drawMode.HasFlag(GameObjectDrawMode.Model) && this.GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.Model))
				this.Draw(scene, scene.GetViewMatrix(), scene.GetProjectionMatrix());

			if (drawMode.HasFlag(GameObjectDrawMode.SpriteBatch) && this.GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.SpriteBatch))
				this.Draw2D(spriteBatch);

			foreach (var obj in this.ChildObjects) obj.DrawLogic(scene, spriteBatch, drawMode);
		}

		public void DrawLogicWithEffect(
			GameScene scene,
			SpriteBatch spriteBatch,
			Effect effect,
			Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams,
			string technique = null,
			GameObjectDrawMode drawMode = GameObjectDrawMode.All)
		{
			if (drawMode.HasFlag(GameObjectDrawMode.Model) && this.GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.Model))
				this.DrawWithSpecificEffect(scene, effect, effectParams, technique);

			if (drawMode.HasFlag(GameObjectDrawMode.SpriteBatch) &&
			    this.GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.SpriteBatch))
				this.Draw2D(spriteBatch);

			foreach (var obj in this.ChildObjects)
				obj.DrawLogicWithEffect(scene, spriteBatch, effect, effectParams, technique, drawMode);
		}

		public virtual void Draw2D(SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		///     Checks if there is a <see cref="Model" /> to draw and draws it.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="view"></param>
		/// <param name="projection"></param>
		public virtual void Draw(GameScene scene, Matrix view, Matrix projection)
		{
			if (this.Model == null) return;


			if (this.Effect == null)
			{
				Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> basicEffectParams =
					(obj, effect, transformationMatrices, mesh, s) =>
					{
						var be = (BasicEffect) effect;
						be.View = view;
						be.World = transformationMatrices[mesh.ParentBone.Index] * obj.WorldMatrix;
						be.Projection = projection;

						this.EffectParams?.Invoke(obj, be, transformationMatrices, mesh, s);
					};

				this.DrawWithSpecificEffect(scene, this.Model.Meshes[0].Effects[0], basicEffectParams, null);
			}
			else
			{

				this.DrawWithSpecificEffect(scene, this.Effect, this.EffectParams, null);

			}
		}

		/// <summary>
		///     Checks if there is a <see cref="Model" /> to draw and draws it with specified Effect.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="effect"></param>
		/// <param name="effectParams"></param>
		/// <param name="technique"></param>
		public virtual void DrawWithSpecificEffect(GameScene scene, Effect effect,
			Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams, string technique = null)
		{
			if (this.Model == null) return; // No model means it can't be rendered.

			// copy the scale of bones from the model to apply it later.
			var transformMatrices = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(transformMatrices);

			var originalRastState = scene.Game.GraphicsDevice.RasterizerState;

			if (!this.CullingEnabled)
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
				foreach (var mesh in this.Model.Meshes)
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
			else
			{
				effect.CurrentTechnique = effect.Techniques[technique];

				foreach (var pass in effect.CurrentTechnique.Passes)
				foreach (var mesh in this.Model.Meshes)
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


			if (!this.CullingEnabled)
				scene.Game.GraphicsDevice.RasterizerState = originalRastState;
		}

		public virtual void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			foreach (var child in this.ChildObjects) child.LoadContent(contentManager, graphicsDevice);

			foreach (var child in this.ChildrenBuffer) child.LoadContent(contentManager, graphicsDevice);
		}

		public virtual void UnloadContent()
		{
			foreach (var child in this.ChildObjects) child.UnloadContent();
		}

		#endregion

		#region Events

		protected event EventHandler<CollisionEventArgs> CollisionEvent;

		public virtual void OnCollision(GameTime gameTime, GameObject collider, GameObject collidable, GameScene scene, Vector3 position,
			Vector3 normal)
		{
			this.OnCollision(new CollisionEventArgs(gameTime, position, normal, collider, collidable, scene));
		}

		public virtual void OnCollision(CollisionEventArgs e)
		{
			this.CollisionEvent?.Invoke(this, e);
		}

		#endregion
	}
	
	[Flags]
	public enum Axis
	{
		X = 0b00000001,
		Y = 0b00000010,
		Z = 0b00000100,
		W = 0b00001000,
		All = ~0
	}
}