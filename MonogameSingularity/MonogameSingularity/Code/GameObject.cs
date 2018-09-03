using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Collisions;
using Singularity.Events;
using Singularity.GameObjects.Interfaces;
using Singularity.Utilities;

namespace Singularity
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

		public bool DrawChildren { get; protected set; } = true;

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
				return ScaleMatrix *
				       RotationMatrix *
				       TranslationMatrix;
			}
		}

		public Matrix TranslationMatrix
		{
			get { return Matrix.CreateTranslation(GetHierarchyPosition()); }
		}

		public Matrix ScaleMatrix
		{
			get { return Matrix.CreateScale(GetHierarchyScale()); }
		}

		public Matrix RotationMatrix
		{
			get
			{
				var rotation = GetHierarchyRotation();

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
			get { return ScaleMatrix * RotationMatrix; }
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
			if (ParentObject == null || !ChildProperties.HasFlag(ChildProperties.Scale)) return Scale;
			return Scale * ParentObject.GetHierarchyScale();
		}

		/// <summary>
		///     Return the added <see cref="Position" /> from this <see cref="GameObject" /> and the <see cref="ParentObject" />
		///     <seealso cref="GetHierarchyPosition()" />
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyPosition()
		{
			if (ParentObject == null) return Position;
			if (ChildProperties.HasFlag(ChildProperties.TranslationRotation))
				return Vector3.Transform(Position, ParentObject.RotationMatrix) + ParentObject.GetHierarchyPosition();
			if (ChildProperties.HasFlag(ChildProperties.Translation)) return Position + ParentObject.GetHierarchyPosition();

			return Position;
		}


		public Vector3 GetHierarchyRotation()
		{
			if (ParentObject == null || !ChildProperties.HasFlag(ChildProperties.Rotation)) return Rotation;
			return Rotation + ParentObject.GetHierarchyRotation();
		}

		public GameObjectDrawMode GetHierarchyDrawMode()
		{
			if (ParentObject == null || !ChildProperties.HasFlag(ChildProperties.DrawMode)) return DrawMode;
			return ParentObject.GetHierarchyDrawMode();
		}


		/// <summary>
		///     Gets all <see cref="Action" />scripts set to this <see cref="GameObject" />
		/// </summary>
		/// <returns></returns>
		public List<Action<GameScene, GameObject, GameTime>> GetScripts()
		{
			return ObjectScripts;
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

			ModelPath = model;

			var center = GetHierarchyPosition();

			// now get max(r + r(v))
			var rm = 0.0f;
			foreach (var mesh in Model.Meshes)
			{
				// get distance
				var bs = mesh.BoundingSphere;
				var dist = (bs.Center - center).Length() + bs.Radius;

				if (dist > rm) rm = dist;
			}

			ModelRadius = rm;

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
			return SetPosition(new Vector3(x, y, 0));
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
			return SetPosition(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets the <see cref="Position" />
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject SetPosition(Vector3 position)
		{
			if (ParentObject != null && ChildProperties.HasFlag(ChildProperties.KeepPositon))
			{
				var mat = Matrix.Identity;

				if (ChildProperties.HasFlag(ChildProperties.Rotation)) mat *= ParentObject.RotationMatrix;
				if (ChildProperties.HasFlag(ChildProperties.Translation)) mat *= ParentObject.TranslationMatrix;

				Position = Vector3.Transform(position, Matrix.Invert(mat));
			}
			else
			{
				Position = position;
			}

			return this;
		}

		public GameObject SetAbsolutePosition(Vector3 position)
		{
			if (ParentObject == null) return this.SetPosition(position);

			var mat = Matrix.Identity;

			if (ChildProperties.HasFlag(ChildProperties.Rotation)) mat *= ParentObject.RotationMatrix;
			if (ChildProperties.HasFlag(ChildProperties.Translation)) mat *= ParentObject.TranslationMatrix;

			Position = Vector3.Transform(position, Matrix.Invert(mat));

			return this;
		}

		public GameObject SetPositionAt(Axis axis, float value)
		{

			if (axis.HasFlag(Axis.X)) SetPosition(value, this.Position.Y, this.Position.Z);
			if (axis.HasFlag(Axis.Y)) SetPosition(this.Position.X, value, this.Position.Z);
			if (axis.HasFlag(Axis.Z)) SetPosition(this.Position.X, this.Position.Y, value);

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
			return AddPosition(new Vector3(x, y, 0));
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
			return AddPosition(new Vector3(x, y, z));
		}

		/// <summary>
		///     Modifies the <see cref="Position" /> by adding the Vectors
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject AddPosition(Vector3 position)
		{
			Position += position;
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
			return AddPosition(new Vector3(x, y, 0), gameTime);
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
			return AddPosition(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     Modifies the <see cref="Position" /> by adding the Vectors multiplied by the deltaTime
		/// </summary>
		/// <param name="position"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddPosition(Vector3 position, GameTime gameTime)
		{

			return AddPosition(position * (float) Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddPositionAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) AddPosition(value, 0, 0);
			if (axis.HasFlag(Axis.Y)) AddPosition(0, value, 0);
			if (axis.HasFlag(Axis.Z)) AddPosition(0, 0, value);

			return this;
		}

		public GameObject AddPositionAt(Axis axis, float value, GameTime gameTime) =>
			AddPositionAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

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
			return SetRotation(new Vector3(x, y, 0));
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
			return SetRotation(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets the <see cref="Rotation" />
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject SetRotation(Vector3 rotation)
		{
			Rotation = rotation;
			return this;
		}
		public GameObject SetRotationAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) SetRotation(value, this.Rotation.Y, this.Rotation.Z);
			if (axis.HasFlag(Axis.Y)) SetRotation(this.Rotation.X, value, this.Rotation.Z);
			if (axis.HasFlag(Axis.Z)) SetRotation(this.Rotation.X, this.Rotation.Y, value);

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
			return AddRotation(new Vector3(x, y, 0));
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
			return AddRotation(new Vector3(x, y, z));
		}

		/// <summary>
		///     Modifies the <see cref="Rotation" /> by adding both <see cref="Vector3" />
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject AddRotation(Vector3 rotation)
		{
			Rotation += rotation;
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
			return AddRotation(new Vector3(x, y, 0), gameTime);
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
			return AddRotation(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="rotation"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddRotation(Vector3 rotation, GameTime gameTime)
		{
			return AddRotation(rotation * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddRotationAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) AddRotation(value, 0, 0);
			if (axis.HasFlag(Axis.Y)) AddRotation(0, value, 0);
			if (axis.HasFlag(Axis.Z)) AddRotation(0, 0, value);

			return this;
		}
		public GameObject AddRotationAt(Axis axis, float value, GameTime gameTime) =>
			AddRotationAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

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
			return SetScale(scale, scale, scale);
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
			return SetScale(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets the <see cref="Scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(Vector3 scale)
		{
			Scale = scale;
			return this;
		}
		public GameObject SetScaleAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) SetScale(value, this.Scale.Y, this.Scale.Z);
			if (axis.HasFlag(Axis.Y)) SetScale(this.Scale.X, value, this.Scale.Z);
			if (axis.HasFlag(Axis.Z)) SetScale(this.Scale.X, this.Scale.Y, value);

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
			return MultiplyScale(new Vector3(x, y, z));
		}

		/// <summary>
		///     Multiplies <see cref="Scale" /> with <paramref name="scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(Vector3 scale)
		{
			Scale *= scale;
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
			return MultiplyScale(new Vector3(x, y, z), gameTime);
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
			
			return MultiplyScale(s);
		}
		public GameObject MultiplyScaleAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) MultiplyScale(value, 1, 1);
			if (axis.HasFlag(Axis.Y)) MultiplyScale(1, value, 1);
			if (axis.HasFlag(Axis.Z)) MultiplyScale(1, 1, value);

			return this;
		}
		public GameObject MultiplyScaleAt(Axis axis, float value, GameTime gameTime) =>
			MultiplyScaleAt(axis, (float) Math.Pow(value, Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate)));

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
			return AddScale(new Vector3(scale));
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
			return AddScale(new Vector3(x, y, z));
		}

		/// <summary>
		///     Adds <paramref name="scale" /> to the <see cref="Scale" />
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(Vector3 scale)
		{
			Scale += scale;
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
			return AddScale(new Vector3(scale), gameTime);
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
			return AddScale(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddScale(Vector3 scale, GameTime gameTime)
		{
			return AddScale(scale * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddScaleAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) AddScale(value, 0, 0);
			if (axis.HasFlag(Axis.Y)) AddScale(0, value, 0);
			if (axis.HasFlag(Axis.Z)) AddScale(0, 0, value);

			return this;
		}
		public GameObject AddScaleAt(Axis axis, float value, GameTime gameTime) =>
			AddScaleAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

		#endregion

		#region SetInertia

		public GameObject SetInertia(float x, float y)
		{
			return SetInertia(x, y, 0);
		}

		public GameObject SetInertia(float x, float y, float z)
		{
			return SetInertia(new Vector3(x, y, z));
		}

		public GameObject SetInertia(Vector3 inertia)
		{
			if (!(this is IInertia)) Console.WriteLine($"Not inheriting IInertia. Inertia should not be used!");


			Inertia = inertia;
			return this;
		}
		public GameObject SetInertiaAt(Axis axis, float value)
		{
			if (axis.HasFlag(Axis.X)) SetInertia(value, this.Inertia.Y, this.Inertia.Z);
			if (axis.HasFlag(Axis.Y)) SetInertia(this.Inertia.X, value, this.Inertia.Z);
			if (axis.HasFlag(Axis.Z)) SetInertia(this.Inertia.X, this.Inertia.Y, value);

			return this;
		}

		#endregion

		#region AddInertia

		public GameObject AddInertia(float x, float y)
		{
			return AddInertia(x, y, 0);
		}

		public GameObject AddInertia(float x, float y, float z)
		{
			return AddInertia(new Vector3(x, y, z));
		}

		public GameObject AddInertia(Vector3 inertia)
		{
			if (!(this is IInertia)) Console.WriteLine($"Not inheriting IInertia. Inertia should not be used!");

			Inertia += inertia;
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
			return AddInertia(new Vector3(x, y, 0), gameTime);
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
			return AddInertia(new Vector3(x, y, z), gameTime);
		}

		/// <summary>
		///     TODO
		/// </summary>
		/// <param name="inertia"></param>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public GameObject AddInertia(Vector3 inertia, GameTime gameTime)
		{
			return AddInertia(inertia * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));
		}
		public GameObject AddInertiaAt(Axis axis, float value)
		{
			switch (axis)
			{
				case Axis.X:
					return AddInertia(value, 0, 0);
				case Axis.Y:
					return AddInertia(0, value, 0);
				case Axis.Z:
					return AddInertia(0, 0, value);
				case Axis.W:
					return this; // doesn't use W axis.
				default:
					throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
			}
		}
		public GameObject AddInertiaAt(Axis axis, float value, GameTime gameTime) =>
			AddInertiaAt(axis, value * (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, SingularityGame.MinimumFramerate));

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
			if (ParentObject == null) return this;

			ParentObject.RemoveChild(this);

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
			ObjectScripts.Add(script);

			return this;
		}

		#endregion

		#region AddCollisionEvent

		public GameObject AddCollisionEvent(Action<CollisionEventArgs> collEvent)
		{
			Debug.Assert(collEvent != null);

			CollisionEvent += (s, e) => collEvent(e);
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
			if (ChildrenBuffer.Contains(child) || ChildObjects.Contains(child))
				return this;

			ChildrenBuffer.Add(child);

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
			if (ChildObjects.Contains(child))
			{
				ChildObjects.Remove(child);
				child.ParentObject = null;

				if (child.ChildProperties.HasFlag(ChildProperties.KeepPositon))
				{
					var mat = Matrix.Identity;

					if (child.ChildProperties.HasFlag(ChildProperties.Rotation)) mat *= RotationMatrix;
					if (child.ChildProperties.HasFlag(ChildProperties.Translation)) mat *= TranslationMatrix;


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
			ChildProperties = properties;

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
			DebugName = name;
			return this;
		}

		#endregion

		#region SetCollision

		public GameObject SetCollision(Collision collision)
		{
			Collision = (Collision) collision.Clone();
			Collision.SetParent(this);
			return this;
		}

		#endregion

		#region SetEnableCollision

		public GameObject SetEnableCollision(bool enable)
		{
			EnablePushCollision = enable;

			return this;
		}

		#endregion

		#region SetEffect

		public GameObject SetEffect(Effect effect, Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams)
		{
			Effect = effect;
			EffectParams = effectParams;
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
			ApplySceneLight = set;
			return this;
		}

		#endregion

		#region SetCulling

		public GameObject SetCulling(bool enabled)
		{
			CullingEnabled = enabled;
			return this;
		}

		#endregion

		#region SetDrawMode

		public GameObject SetDrawMode(GameObjectDrawMode drawMode)
		{
			DrawMode = drawMode;
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

		private Vector3 BeginUpdatePosition;

		/// <summary>
		///     Calls <seealso cref="Update" />, and calls back to the scene.
		///     After that all <see cref="ChildObjects" /> will be updated.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public void UpdateLogic(GameScene scene, GameTime gameTime)
		{
			// save update position
			this.BeginUpdatePosition = this.GetHierarchyPosition();

			// begin updating children
			foreach (var obj in ChildObjects.ToArray())
				obj.UpdateLogic(scene, gameTime);

			// This update procedure

			// copy ChildrenBuffer to array to allow new children
			var cbArray = ChildrenBuffer.ToArray();
			ChildrenBuffer.Clear();

			// invoke update method
			Update(scene, gameTime);

			// add previously buffered children to the ChildObjects
			ChildObjects.AddRange(cbArray);

			// check if we're an inertia object
			// add inertia.
			if (this is IInertia)
				AddPosition(this.Inertia, gameTime);

			// execute scripts
			foreach (var actionScript in ObjectScripts) actionScript(scene, this, gameTime);


			this.MoveInOctree(scene, gameTime, true);
			
			// update Child Positions in Octree
			foreach (var obj in ChildObjects.ToArray())
				obj.MoveInOctree(scene, gameTime, false);
			
		}

		private void MoveInOctree(GameScene scene, GameTime gameTime, Boolean checkCollision)
		{
			if (checkCollision)
				scene.HandleCollision(gameTime, this, GetHierarchyPosition());

			var cPosition = this.GetHierarchyPosition();
			if (cPosition  != this.BeginUpdatePosition)
			{
				scene.MoveOctree(this, this.BeginUpdatePosition);
				this.BeginUpdatePosition = cPosition;
			}


			// if we are allowed to move the camera, do it

			scene.CameraLocked = false;
			if (this is ICameraController controller) controller.SetCamera(scene);
			scene.CameraLocked = true;
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
			if (drawMode.HasFlag(GameObjectDrawMode.Model) && GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.Model))
				Draw(scene, scene.GetViewMatrix(), scene.GetProjectionMatrix());

			if (drawMode.HasFlag(GameObjectDrawMode.SpriteBatch) && GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.SpriteBatch))
				Draw2D(spriteBatch);

			if(DrawChildren)
				foreach (var obj in ChildObjects) obj.DrawLogic(scene, spriteBatch, drawMode);
		}

		public void DrawLogicWithEffect(
			GameScene scene,
			SpriteBatch spriteBatch,
			Effect effect,
			Action<GameObject, Effect, Matrix[], ModelMesh, GameScene> effectParams,
			string technique = null,
			GameObjectDrawMode drawMode = GameObjectDrawMode.All)
		{
			if (drawMode.HasFlag(GameObjectDrawMode.Model) && GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.Model))
				DrawWithSpecificEffect(scene, effect, effectParams, technique);

			if (drawMode.HasFlag(GameObjectDrawMode.SpriteBatch) &&
			    GetHierarchyDrawMode().HasFlag(GameObjectDrawMode.SpriteBatch))
				Draw2D(spriteBatch);

			if(DrawChildren)
				foreach (var obj in ChildObjects)
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
			if (Model == null) return;


			if (Effect == null)
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

				DrawWithSpecificEffect(scene, Model.Meshes[0].Effects[0], basicEffectParams, null);
			}
			else
			{

				DrawWithSpecificEffect(scene, Effect, EffectParams, null);

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
			if (Model == null) return; // No model means it can't be rendered.

			// copy the scale of bones from the model to apply it later.
			var transformMatrices = new Matrix[Model.Bones.Count];
			Model.CopyAbsoluteBoneTransformsTo(transformMatrices);

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
			else
			{
				effect.CurrentTechnique = effect.Techniques[technique];

				foreach (var pass in effect.CurrentTechnique.Passes)
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


			if (!CullingEnabled)
				scene.Game.GraphicsDevice.RasterizerState = originalRastState;
		}

		public virtual void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			foreach (var child in ChildObjects) child.LoadContent(contentManager, graphicsDevice);

			foreach (var child in ChildrenBuffer) child.LoadContent(contentManager, graphicsDevice);
		}

		public virtual void UnloadContent()
		{
			foreach (var child in ChildObjects) child.UnloadContent();
		}

		#endregion

		#region Events

		protected event EventHandler<CollisionEventArgs> CollisionEvent;

		public virtual void OnCollision(GameTime gameTime, GameObject collider, GameObject collidable, GameScene scene, Vector3 position,
			Vector3 normal)
		{
			OnCollision(new CollisionEventArgs(gameTime, position, normal, collider, collidable, scene));
		}

		public virtual void OnCollision(CollisionEventArgs e)
		{
			CollisionEvent?.Invoke(this, e);
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