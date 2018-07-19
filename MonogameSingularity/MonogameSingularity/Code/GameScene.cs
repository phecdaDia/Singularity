using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Collisions;
using Singularity.GameObjects.Interfaces;
using Singularity.Utilities;

namespace Singularity
{
	public abstract class GameScene
	{
		private readonly IList<GameObject>
			BufferedObjects; // This Dictionary is used when adding new GameObjects to the Scene.

		private readonly Octree<GameObject> ColliderObjects; // all current GameObjects in the scene.
		protected float MaximumCullingDistance = 100.0f;

		protected float MinimumCullingDistance = 0.05f;

		private bool UseAbsoluteCameraTarget;


		/// <summary>
		///     Creates a new <see cref="GameScene" />
		/// </summary>
		/// <param name="game"></param>
		/// <param name="sceneKey">Unique key for <seealso cref="SceneManager" /></param>
		/// <param name="sceneSize">Size of the scene in 2^x</param>
		/// <param name="minPartition">Minimum size of <seealso cref="Octree{T}" /> partitioning</param>
		/// <param name="precision">Buffer radius for <seealso cref="Octree{T}" /></param>
		public GameScene(SingularityGame game, string sceneKey, int sceneSize = 16, int minPartition = 2,
			float precision = 0.0f)
		{
			Game = game;

			SceneKey = sceneKey;

			// Setting default values for all members
			CameraPosition = new Vector3();
			//this.ColliderObjects = new Dictionary<Type, IList>();
			BufferedObjects = new List<GameObject>();

			ColliderObjects = new Octree<GameObject>(sceneSize, minPartition, precision);
		}

		public SingularityGame Game { get; }

		public Vector3 CameraPosition { get; private set; } // Current position of the camera
		public Vector3 CameraTarget { get; private set; } // Current view direction of the camera
		public string SceneKey { get; }

		public bool CameraLocked { get; set; }

		/// <summary>
		///     Clears all <seealso cref="GameObject" /> and calls <see cref="AddGameObjects" />
		/// </summary>
		public void SetupScene(GameScene previousScene, int entranceId)
		{
			UnloadContent();
			// clear all current objects.
			ColliderObjects.Clear();

			// Now setup the new objects
			AddGameObjects(previousScene, entranceId);
		}

		/// <summary>
		///     Spawns a new <seealso cref="GameObject" /> on the next frame.
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="parent"></param>
		/// <param name="properties"></param>
		public void SpawnObject(GameObject gameObject, GameObject parent = null,
			ChildProperties properties = ChildProperties.All)
		{
			if (parent != null)
				gameObject.SetParent(parent, properties);
			BufferedObjects.Add(gameObject);
		}

		/// <summary>
		///     Moves an <seealso cref="GameObject" />
		///     This is required for proper collision detection!
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="previousPosition"></param>
		public void MoveOctree(GameObject gameObject, Vector3 previousPosition)
		{
			//this.ColliderObjects.RemoveObject(gameObject, previousPosition);
			//this.AddObject(gameObject);

			ColliderObjects.MoveObject(gameObject, gameObject.ModelRadius, previousPosition, gameObject.GetHierarchyPosition());
		}

		public void RemoveObject(GameObject gameObject)
		{
			gameObject.UnloadContent();
			ColliderObjects.RemoveObject(gameObject, gameObject.GetHierarchyPosition());
		}

		/// <summary>
		///		DEPRECATED
		///     Adds all <seealso cref="GameObject" />
		/// </summary>
		[Obsolete("Please update to include GameScene as first parameter", true)]
		protected virtual void AddGameObjects(int entranceId) { }

		/// <summary>
		///     Adds all <seealso cref="GameObject" />
		/// </summary>
		/// <param name="previousScene"></param>
		/// <param name="entranceId"></param>
		protected abstract void AddGameObjects(GameScene previousScene, int entranceId);


		protected void AddObject(IEnumerable<GameObject> gameObjects)
		{
			foreach (var go in gameObjects)
			{
				AddObject(go);
			}
		}

		/// <summary>
		///     Adds a <seealso cref="GameObject" />
		/// </summary>
		/// <param name="gameObject"></param>
		protected void AddObject(GameObject gameObject)
		{
			foreach (var children in gameObject.ChildObjects) AddObject(children);

			gameObject.LoadContent(this.Game.Content, this.Game.GraphicsDevice);

			if (gameObject.Collision == null)
			{
				ColliderObjects.AddObject(gameObject, gameObject.Position, 0.0f);
				return;
			}

			var scale = gameObject.GetHierarchyScale();
			var maxScale = Math.Max(Math.Max(scale.X, scale.Y), scale.Z);

			if (gameObject.Collision.GetType() == typeof(SphereCollision))
				ColliderObjects.AddObject(gameObject, gameObject.Position,
					maxScale * (gameObject.Collision as SphereCollision).Radius);
			else
				ColliderObjects.AddObject(gameObject, gameObject.Position, maxScale * gameObject.ModelRadius);
		}

		/// <summary>
		///     Sets relative <see cref="CameraPosition" /> and <see cref="CameraTarget" />
		///     Calls <seealso cref="SetCamera" /> and <seealso cref="SetCameraTarget" />
		/// </summary>
		/// <param name="cameraPosition"></param>
		/// <param name="cameraTarget"></param>
		public void SetCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			SetCameraPosition(cameraPosition);
			SetCameraTarget(cameraTarget);
		}

		/// <summary>
		///     Sets absolute <see cref="CameraPosition" /> and <see cref="CameraTarget" />
		///     Calls <seealso cref="SetCamera" /> and <seealso cref="SetAbsoluteCameraTarget" />
		/// </summary>
		/// <param name="cameraPosition"></param>
		/// <param name="cameraTarget"></param>
		public void SetAbsoluteCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			SetCameraPosition(cameraPosition);
			SetAbsoluteCameraTarget(cameraTarget);
		}

		/// <summary>
		///     Sets <see cref="CameraPosition" />
		/// </summary>
		/// <param name="cameraPosition"></param>
		public void SetCameraPosition(Vector3 cameraPosition)
		{
			if (CameraLocked)
			{
				Console.WriteLine($"Camera has been locked. Please refer to ICameraController.SetCamera to set the camera!");
				return; // camera is locked!
			}

			CameraPosition = cameraPosition;
		}


		/// <summary>
		///     Sets <see cref="CameraPosition" />
		/// </summary>
		public void SetCameraPosition(float x, float y, float z)
		{
			SetCameraPosition(new Vector3(x, y, z));
		}

		/// <summary>
		///     Sets relative <see cref="CameraTarget" />
		/// </summary>
		/// <param name="cameraTarget"></param>
		public void SetCameraTarget(Vector3 cameraTarget)
		{
			if (CameraLocked)
			{
				Console.WriteLine($"Camera has been locked. Please refer to ICameraController.SetCamera to set the camera!");
				return; // camera is locked!
			}

			UseAbsoluteCameraTarget = false;
			CameraTarget = cameraTarget;
		}

		/// <summary>
		///     Sets absolute <see cref="CameraTarget" />
		/// </summary>
		/// <param name="cameraTarget"></param>
		public void SetAbsoluteCameraTarget(Vector3 cameraTarget)
		{
			if (CameraLocked)
			{
				Console.WriteLine($"Camera has been locked. Please refer to ICameraController.SetCamera to set the camera!");
				return; // camera is locked!
			}

			UseAbsoluteCameraTarget = true;
			CameraTarget = cameraTarget;
		}

		/// <summary>
		///     Sets <see cref="MinimumCullingDistance" /> and <see cref="MaximumCullingDistance" />
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		public void SetCullingDistance(float c1, float c2)
		{
			MinimumCullingDistance = c1;
			MaximumCullingDistance = c2;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="safePosition"></param>
		public void HandleCollision(GameObject gameObject, Vector3 safePosition)
		{
			if (!(gameObject is ICollider)) return;

			var collisionFixes = 0;

			bool didCollide = false;


			var collidables = ColliderObjects.GetObjects(gameObject.Position, go => go is ICollidable && go != gameObject);
			do
			{
				didCollide = false;
				if (++collisionFixes >= 2) return;
				// get list of collidables.

				foreach (var go in collidables)
					CollisionManager.DoesCollide(gameObject.Collision, go.Collision,
						(collider, collidable, pos, nor) =>
						{
							//Console.WriteLine("Collision");

							if (gameObject.EnablePushCollision)
								gameObject.SetPosition(CollisionManager.HandleCollision(collider, collidable, pos, nor));

							gameObject.OnCollision(gameObject, go, this, pos, nor);
							go.OnCollision(gameObject, go, this, pos, nor);

							didCollide = true;
						});
			} while (didCollide && gameObject.EnablePushCollision);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="ray"></param>
		/// <returns></returns>
		public RayCollisionPoint CollideRay(Ray ray, Func<GameObject, Boolean> predicate = null)
		{
			// get all objects
			List<GameObject> collidables = predicate == null ? 
				ColliderObjects.GetAllObjects(go => go is ICollidable) : 
				ColliderObjects.GetAllObjects(go => predicate(go) && go is ICollidable);

			var nearestCollision = new RayCollisionPoint();

			foreach (var collidable in collidables)
			{
				//Console.WriteLine($"Testing RCP with {collidable}");

				var rcp = CollisionManager.GetRayCollision(ray, collidable.Collision);

				// check if the point is closer
				if (rcp.DidCollide && rcp.RayDistance < nearestCollision.RayDistance && rcp.RayDistance >= 0.0f)
					nearestCollision = rcp;
			}


			return nearestCollision;
		}

		/// <summary>
		///     Creates a <seealso cref="Matrix" /> with all camera options set.
		/// </summary>
		/// <returns></returns>
		public virtual Matrix GetViewMatrix()
		{
			return Matrix.CreateLookAt(CameraPosition,
				UseAbsoluteCameraTarget ? CameraTarget : CameraPosition + 5f * CameraTarget, Vector3.Up);
		}

		/// <summary>
		///     Creates a <seealso cref="Matrix" />
		/// </summary>
		/// <returns></returns>
		public virtual Matrix GetProjectionMatrix()
		{
			return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, MinimumCullingDistance,
				MaximumCullingDistance);
		}

		///// <summary>
		///// TODO
		///// </summary>
		public virtual void LoadContent()
		{}

		/// <summary>
		/// TODO
		/// </summary>
		public virtual void UnloadContent()
		{
			foreach (var obj in ColliderObjects.GetAllObjects()) obj.UnloadContent();
		}

		public List<GameObject> GetAllObjects(Func<GameObject, bool> predicate = null)
		{
			return ColliderObjects.GetAllObjects(predicate);
		}


		/// <summary>
		///     Updates all <seealso cref="GameObject" /> and adds <see cref="BufferedObjects" />
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			var objs = ColliderObjects.GetAllObjects().ToArray();
			//Console.WriteLine($"{objs.Length} objects in the octree.");

			//Console.WriteLine($"{objs.Length}");

			foreach (var obj in objs)
				if (obj.ParentObject == null)
					obj.UpdateLogic(this, gameTime);

			// add our buffered objects
			foreach (var obj in BufferedObjects)
				AddObject(obj);

			// clear buffers
			BufferedObjects.Clear();
		}

		/// <summary>
		///     Draws all <seealso cref="GameObject" />
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="finalTarget"></param>
		public virtual void Draw(SpriteBatch spriteBatch, RenderTarget2D finalTarget)
		{
			// render it on our temporary rendertarget first
			// will be used later for shadows.
			Game.GraphicsDevice.SetRenderTarget(finalTarget);

			spriteBatch.Begin(SpriteSortMode.FrontToBack); // allows for better 2d drawing.

			Game.GraphicsDevice.Clear(Color.Transparent); // sets everything to transparent, clears the entire RenderTarget

			foreach (var obj in ColliderObjects.GetAllObjects(o => o.ParentObject == null)) obj.DrawLogic(this, spriteBatch);


			spriteBatch.End();
		}

		#region Events

		/// <summary>
		/// TODO
		/// </summary>
		protected event EventHandler<EventArgs> ScenePauseEvent;

		/// <summary>
		/// TODO
		/// </summary>
		public void OnScenePause() => ScenePauseEvent?.Invoke(this, EventArgs.Empty);

		/// <summary>
		/// TODO
		/// </summary>
		protected event EventHandler<EventArgs> SceneResumeEvent;

		/// <summary>
		/// TODO
		/// </summary>
		public void OnSceneResume() => SceneResumeEvent?.Invoke(this, EventArgs.Empty);

		#endregion
	}

	/// <summary>
	/// TODO
	/// </summary>
	public interface ITransparent
	{
	}
}