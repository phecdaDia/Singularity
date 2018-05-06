using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code.Collisions;
using Singularity.Code.GameObjects;
using Singularity.Code.Utilities;

namespace Singularity.Code
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

		private float MinimumCullingDistance = 0.05f;
		private float MaximumCullingDistance = 100.0f;


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
		public void SetupScene()
		{
			// clear all current objects.
			this.ColliderObjects.Clear();

			// Now setup the new objects
			AddGameObjects();
		}

		/// <summary>
		/// Spawns a new <seealso cref="GameObject"/> on the next frame.
		/// </summary>
		/// <param name="gameObject"></param>
		public void SpawnObject(GameObject gameObject)
		{
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
			this.ColliderObjects.RemoveObject(gameObject, previousPosition);
			this.AddObject(gameObject);
		}

		/// <summary>
		/// Adds all <seealso cref="GameObject"/>
		/// </summary>
		protected abstract void AddGameObjects();

		/// <summary>
		/// Adds a <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="gameObject"></param>
		protected void AddObject(GameObject gameObject)
		{
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
			this.CameraPosition = cameraPosition;
		}

		/// <summary>
		/// Sets relative <see cref="CameraTarget"/>
		/// </summary>
		/// <param name="cameraTarget"></param>
		public void SetCameraTarget(Vector3 cameraTarget)
		{
			this.UseAbsoluteCameraTarget = false;
			this.CameraTarget = cameraTarget;
		}

		/// <summary>
		/// Sets absolute <see cref="CameraTarget"/>
		/// </summary>
		/// <param name="cameraTarget"></param>
		public void SetAbsoluteCameraTarget(Vector3 cameraTarget)
		{
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

			List<GameObject> colliders = new List<GameObject>();

			int collisionFixes = 0;

			Boolean DidCollide = false;

			do
			{
				DidCollide = false;
				//if (++collisionFixes >= 2)
				//{
				//	// couldn't escape collision after n tries. Escaping to a safe position
				//	gameObject.SetPosition(safePosition);
				//	return;

				//}
				// get list of collidables.
				
				colliders = ColliderObjects.GetObjects(gameObject.Position, go => go is ICollidable && go != gameObject);

				foreach (var go in colliders)
				{

					if (CollisionManager.DoesCollide(gameObject.Collision, go.Collision, out var position, out var normal))
					{
						gameObject.SetPosition(CollisionManager.HandleCollision(gameObject, go, position, normal));
						DidCollide = true;
					}

				}



			} while (DidCollide);

		}

		/// <summary>
		/// Creates a <seealso cref="Matrix"/> with all camera options set.
		/// </summary>
		/// <returns></returns>
		public Matrix GetViewMatrix()
		{
			Vector3 targetVector =
				this.UseAbsoluteCameraTarget ? this.CameraTarget : this.CameraPosition + 5f * this.CameraTarget;

			// transform camera position because of some black magic
			var viewPosition = new Vector3(this.CameraPosition.X, this.CameraPosition.Y, this.CameraPosition.Z);


			var viewTarget = new Vector3(targetVector.X, targetVector.Y, targetVector.Z);

			//Console.WriteLine($"VM: {viewPosition} {viewTarget}");

			return Matrix.CreateLookAt(viewPosition, viewTarget, Vector3.Up);

		}

		/// <summary>
		/// Creates a <seealso cref="Matrix"/>
		/// </summary>
		/// <returns></returns>
		public Matrix GetProjectionMatrix()
		{
			return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, this.MinimumCullingDistance, this.MaximumCullingDistance);
		}

		/// <summary>
		/// Adds lightning to the <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="effect"></param>
		public abstract void AddLightningToEffect(BasicEffect effect);


		/// <summary>
		/// Updates all <seealso cref="GameObject"/> and adds <see cref="BufferedObjects"/>
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{

			foreach (GameObject obj in this.ColliderObjects.GetAllObjects().ToArray()) obj.UpdateLogic(this, gameTime);

			// add our buffered objects
			foreach (GameObject obj in BufferedObjects) this.AddObject(obj);

			// clear buffers
			this.BufferedObjects.Clear();
		}

		/// <summary>
		/// Draws all <seealso cref="GameObject"/>
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (GameObject obj in this.ColliderObjects.GetAllObjects()) obj.DrawLogic(this, spriteBatch);
		}

	}
}
