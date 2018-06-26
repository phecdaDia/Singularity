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
		public SingularityGame Game { get; private set; }

		public Vector3 CameraPosition { get; private set; }     // Current position of the camera
		public Vector3 CameraTarget { get; private set; }       // Current view direction of the camera
		private Octree<GameObject> ColliderObjects;               // all current GameObjects in the scene.

		private IList<GameObject> BufferedObjects;                     // This Dictionary is used when adding new GameObjects to the Scene.
		public String SceneKey { get; }

		private Boolean UseAbsoluteCameraTarget = false;

		protected float MinimumCullingDistance = 0.05f;
		protected float MaximumCullingDistance = 100.0f;

		public Boolean CameraLocked { get; set; }


		/// <summary>
		/// Creates a new <see cref="GameScene"/>
		/// </summary>
		/// <param name="sceneKey">Unique key for <seealso cref="SceneManager"/></param>
		/// <param name="sceneSize">Size of the scene in 2^x</param>
		/// <param name="minPartition">Minimum size of <seealso cref="Octree{T}"/> partitioning</param>
		/// <param name="precision">Buffer radius for <seealso cref="Octree{T}"/></param>
		public GameScene(SingularityGame game, String sceneKey, int sceneSize = 16, int minPartition = 2, float precision = 0.0f)
		{
			this.Game = game;

			this.SceneKey = sceneKey;

			// Setting default values for all members
			this.CameraPosition = new Vector3();
			//this.ColliderObjects = new Dictionary<Type, IList>();
			this.BufferedObjects = new List<GameObject>();

			this.ColliderObjects = new Octree<GameObject>(sceneSize, minPartition, precision);
		}

		/// <summary>
		/// Clears all <seealso cref="GameObject"/> and calls <see cref="AddGameObjects"/>
		/// </summary>
		public void SetupScene(int entranceId)
		{
			UnloadContent();
			// clear all current objects.
			this.ColliderObjects.Clear();
			
			// Now setup the new objects
			AddGameObjects(entranceId);
		}

		/// <summary>
		/// Spawns a new <seealso cref="GameObject"/> on the next frame.
		/// </summary>
		/// <param name="gameObject"></param>
		public void SpawnObject(GameObject gameObject, GameObject parent = null)
		{
			if (parent != null)
				gameObject.SetParent(parent);
			this.BufferedObjects.Add(gameObject);
		}

		/// <summary>
		/// Moves an <seealso cref="GameObject"/>
		/// This is required for proper collision detection!
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="previousPosition"></param>
		public void MoveOctree(GameObject gameObject, Vector3 previousPosition)
		{
			//this.ColliderObjects.RemoveObject(gameObject, previousPosition);
			//this.AddObject(gameObject);

			this.ColliderObjects.MoveObject(gameObject, gameObject.ModelRadius, previousPosition, gameObject.GetHierarchyPosition());
		}

		public void RemoveObject(GameObject gameObject)
		{
			gameObject.UnloadContent();
			this.ColliderObjects.RemoveObject(gameObject, gameObject.GetHierarchyPosition());
		}

		/// <summary>
		/// Adds all <seealso cref="GameObject"/>
		/// </summary>
		protected abstract void AddGameObjects(int entranceId);

		/// <summary>
		/// Adds a <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="gameObject"></param>
		protected void AddObject(GameObject gameObject)
		{
			foreach (var children in gameObject.ChildObjects)
			{
				AddObject(children);
			}


			if (gameObject.Collision == null)
			{
				this.ColliderObjects.AddObject(gameObject, gameObject.Position, 0.0f);
				return;
			}

			Vector3 scale = gameObject.GetHierarchyScale();
			float maxScale = Math.Max(Math.Max(scale.X, scale.Y), scale.Z);
			
			if (gameObject.Collision.GetType() == typeof(SphereCollision))
				this.ColliderObjects.AddObject(gameObject, gameObject.Position, maxScale * (gameObject.Collision as SphereCollision).Radius);
			else
				this.ColliderObjects.AddObject(gameObject, gameObject.Position, maxScale * gameObject.ModelRadius);
			return;
		}

		/// <summary>
		/// Sets relative <see cref="CameraPosition"/> and <see cref="CameraTarget"/>
		/// Calls <seealso cref="SetCamera"/> and <seealso cref="SetCameraTarget"/>
		/// </summary>
		/// <param name="cameraPosition"></param>
		/// <param name="cameraTarget"></param>
		public void SetCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			SetCameraPosition(cameraPosition);
			SetCameraTarget(cameraTarget);
		}

		/// <summary>
		/// Sets absolute <see cref="CameraPosition"/> and <see cref="CameraTarget"/>
		/// Calls <seealso cref="SetCamera"/> and <seealso cref="SetAbsoluteCameraTarget"/>
		/// </summary>
		/// <param name="cameraPosition"></param>
		/// <param name="cameraTarget"></param>
		public void SetAbsoluteCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			SetCameraPosition(cameraPosition);
			SetAbsoluteCameraTarget(cameraTarget);
		}

		/// <summary>
		/// Sets <see cref="CameraPosition"/>
		/// </summary>
		/// <param name="cameraPosition"></param>
		public void SetCameraPosition(Vector3 cameraPosition)
		{
			if (CameraLocked)
			{
				Console.WriteLine($"Camera has been locked. Please refer to ICameraController.SetCamera to set the camera!");
				return; // camera is locked!
			}

			this.CameraPosition = cameraPosition;
		}


		/// <summary>
		/// Sets <see cref="CameraPosition"/>
		/// </summary>
		public void SetCameraPosition(float x, float y, float z) => this.SetCameraPosition(new Vector3(x, y, z));

		/// <summary>
		/// Sets relative <see cref="CameraTarget"/>
		/// </summary>
		/// <param name="cameraTarget"></param>
		public void SetCameraTarget(Vector3 cameraTarget)
		{
			if (CameraLocked)
			{
				Console.WriteLine($"Camera has been locked. Please refer to ICameraController.SetCamera to set the camera!");
				return; // camera is locked!
			}

			this.UseAbsoluteCameraTarget = false;
			this.CameraTarget = cameraTarget;
		}

		/// <summary>
		/// Sets absolute <see cref="CameraTarget"/>
		/// </summary>
		/// <param name="cameraTarget"></param>
		public void SetAbsoluteCameraTarget(Vector3 cameraTarget)
		{
			if (CameraLocked)
			{
				Console.WriteLine($"Camera has been locked. Please refer to ICameraController.SetCamera to set the camera!");
				return; // camera is locked!
			}

			this.UseAbsoluteCameraTarget = true;
			this.CameraTarget = cameraTarget;
		}

		/// <summary>
		/// Sets <see cref="MinimumCullingDistance"/> and <see cref="MaximumCullingDistance"/>
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		public void SetCullingDistance(float c1, float c2)
		{
			this.MinimumCullingDistance = c1;
			this.MaximumCullingDistance = c2;
		}

		public void HandleCollision(GameObject gameObject, Vector3 safePosition)
		{
			if (!(gameObject is ICollider)) return;

			int collisionFixes = 0;

			Boolean DidCollide = false;

			do
			{
				DidCollide = false;
				if (++collisionFixes >= 2)
				{
					//Console.WriteLine($"Could not fix collision!");
					// couldn't escape collision after n tries. Escaping to a safe position
					//gameObject.SetPosition(safePosition);
					return;

				}
				// get list of collidables.

				var collidables = ColliderObjects.GetObjects(gameObject.Position, go => go is ICollidable && go != gameObject);

				foreach (var go in collidables)
				{

					CollisionManager.DoesCollide(gameObject.Collision, go.Collision,
					(collider, collidable, pos, nor) =>
					{
						//Console.WriteLine("Collision");

						if (gameObject.EnablePushCollision)
							gameObject.SetPosition(CollisionManager.HandleCollision(collider, collidable, pos, nor));
						
						gameObject.OnCollision(gameObject, go, this, pos, nor);
						go.OnCollision(gameObject, go, this, pos, nor);

						DidCollide = true;
					});

				}

			} while (DidCollide && gameObject.EnablePushCollision);

		}

		public RayCollisionPoint CollideRay(Ray ray)
		{
			// get all objects
			var collidables = ColliderObjects.GetAllObjects((GameObject go) => go is ICollidable);

			var nearestCollision = new RayCollisionPoint();

			foreach (var collidable in collidables)
			{
				//Console.WriteLine($"Testing RCP with {collidable}");

				RayCollisionPoint rcp = CollisionManager.GetRayCollision(ray, collidable.Collision);

				// check if the point is closer
				if (rcp.DidCollide && rcp.RayDistance < nearestCollision.RayDistance && rcp.RayDistance >= 0.0f)
				{
					nearestCollision = rcp;
				}
			}


			return nearestCollision;
		}

		/// <summary>
		/// Creates a <seealso cref="Matrix"/> with all camera options set.
		/// </summary>
		/// <returns></returns>
		public virtual Matrix GetViewMatrix()
		{
			return Matrix.CreateLookAt(this.CameraPosition, this.UseAbsoluteCameraTarget ? this.CameraTarget : this.CameraPosition + 5f * this.CameraTarget, Vector3.Up);

		}

		/// <summary>
		/// Creates a <seealso cref="Matrix"/>
		/// </summary>
		/// <returns></returns>
		public virtual Matrix GetProjectionMatrix()
		{
			return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, this.MinimumCullingDistance, this.MaximumCullingDistance);
		}

		// deprecated

		///// <summary>
		///// Adds lightning to the <seealso cref="GameObject"/>
		///// </summary>
		///// <param name="effect"></param>
		//public abstract void AddLightningToEffect(Effect effect);

		public virtual void LoadContent()
		{
			// load content
			foreach (var obj in ColliderObjects.GetAllObjects())
			{
				obj.LoadContent(this.Game.Content, this.Game.GraphicsDevice);
			}
		}

		public virtual void UnloadContent()
		{
			foreach (var obj in ColliderObjects.GetAllObjects())
			{
				obj.UnloadContent();
			}
		}

		public List<GameObject> GetAllObjects(Func<GameObject, Boolean> predicate = null)
		{
			return this.ColliderObjects.GetAllObjects(predicate);
		}


		/// <summary>
		/// Updates all <seealso cref="GameObject"/> and adds <see cref="BufferedObjects"/>
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			var objs = this.ColliderObjects.GetAllObjects(o => o.ParentObject == null) .ToArray();
			//Console.WriteLine($"{objs.Length} objects in the octree.");

			//Console.WriteLine($"{objs.Length}");

			foreach (GameObject obj in objs) obj.UpdateLogic(this, gameTime);

			// add our buffered objects
			foreach (GameObject obj in BufferedObjects)
			{
				obj.LoadContent(this.Game.Content, this.Game.GraphicsDevice);
				this.AddObject(obj);
			}

			// clear buffers
			this.BufferedObjects.Clear();
		}

		/// <summary>
		/// Draws all <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="spriteBatch"></param>
		public virtual void Draw(SpriteBatch spriteBatch, RenderTarget2D finalTarget)
		{
			// render it on our temporary rendertarget first
			// will be used later for shadows.
			Game.GraphicsDevice.SetRenderTarget(finalTarget);

			spriteBatch.Begin(SpriteSortMode.FrontToBack);  // allows for better 2d drawing.

			Game.GraphicsDevice.Clear(Color.Transparent);   // sets everything to transparent, clears the entire RenderTarget

			foreach (GameObject obj in this.ColliderObjects.GetAllObjects(o => o.ParentObject == null)) obj.DrawLogic(this, spriteBatch);


			spriteBatch.End();
		}


		protected event EventHandler<EventArgs> ScenePauseEvent;

		public virtual void OnScenePause() => ScenePauseEvent?.Invoke(this, EventArgs.Empty);

		protected event EventHandler<EventArgs> SceneResumeEvent;

		public virtual void OnSceneResume() => SceneResumeEvent?.Invoke(this, EventArgs.Empty);

	}

	public interface ITransparent { }
}
