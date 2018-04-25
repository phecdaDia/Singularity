using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code.Enum;
using Singularity.Code.Utilities;

namespace Singularity.Code
{
	public abstract class GameScene
	{
		public Vector3 CameraPosition { get; private set; }     // Current position of the camera
		public Vector3 CameraTarget { get; private set; }       // Current view direction of the camera
		private Octree<GameObject> ColliderObjects;               // all current GameObjects in the scene.
		private List<GameObject> ActorObjects;

		private IList<GameObject> BufferedActors;                     // This Dictionary is used when adding new GameObjects to the Scene.
		private IList<GameObject> BufferedColliders;                     // This Dictionary is used when adding new GameObjects to the Scene.
		public String SceneKey { get; }

		private Boolean UseAbsoluteCameraTarget = false;

		private float MinimumCullingDistance = 0.05f;
		private float MaximumCullingDistance = 100.0f;



		public GameScene(String sceneKey, int sceneSize = 16, int minPartition = -2)
		{
			this.SceneKey = sceneKey;

			// Setting default values for all members
			this.CameraPosition = new Vector3();
			//this.ColliderObjects = new Dictionary<Type, IList>();
			this.BufferedActors = new List<GameObject>();
			this.BufferedColliders = new List<GameObject>();

			this.ColliderObjects = new Octree<GameObject>(sceneSize, minPartition);
			this.ActorObjects = new List<GameObject>();
		}

		public void SetupScene()
		{
			// clear all current objects.
			this.ColliderObjects.Clear();

			// Now setup the new objects
			AddGameObjects();
		}

		public void SpawnActor(GameObject gameObject)
		{
			this.BufferedActors.Add(gameObject);
		}

		public void SpawnCollider(GameObject gameObject)
		{
			this.BufferedColliders.Add(gameObject);
		}

		protected abstract void AddGameObjects();

		protected void AddActor(GameObject gameObject)
		{
			this.ActorObjects.Add(gameObject);
		}

		protected void AddCollider(GameObject gameObject)
		{
			if (gameObject.Model == null)
			{
				this.ColliderObjects.AddObject(gameObject, 0.0f, gameObject.Position);
				return;
			}

			BoundingSphere[] spheres = new BoundingSphere[gameObject.Model.Meshes.Count];
			for (int i = 0; i < gameObject.Model.Meshes.Count; i++)
			{
				spheres[i] = gameObject.Model.Meshes[i].BoundingSphere;
			}

			Vector3 scale = gameObject.GetHierarchyScale();

			this.ColliderObjects.AddObject(gameObject, gameObject.GetHierarchyPosition(), Math.Max(Math.Max(scale.X, scale.Y), scale.Z), spheres);
		}

		public Boolean DoesCollide(Vector3 position, float radius)
		{
			//return false;
			// get some colliders from the octree
			//this.ColliderObjects.GetObjects(position)

			BoundingSphere bs = new BoundingSphere(position, radius);

			List<GameObject> collidables = this.ColliderObjects.GetObjects(position);


			foreach (var go in collidables)
			{
				if (go.DoesCollide(bs)) return true;

			}

			return false;

		}

		public void SetCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			SetCameraPosition(cameraPosition);
			SetCameraTarget(cameraTarget);
		}
		public void SetAbsoluteCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			SetCameraPosition(cameraPosition);
			SetAbsoluteCameraTarget(cameraTarget);
		}
		public void SetCameraPosition(Vector3 cameraPosition)
		{
			this.CameraPosition = cameraPosition;
		}

		public void SetCameraTarget(Vector3 cameraTarget)
		{
			this.UseAbsoluteCameraTarget = false;
			this.CameraTarget = cameraTarget;
		}

		public void SetAbsoluteCameraTarget(Vector3 cameraTarget)
		{
			this.UseAbsoluteCameraTarget = true;
			this.CameraTarget = cameraTarget;
		}

		public void SetCullingDistance(float c1, float c2)
		{
			this.MinimumCullingDistance = c1;
			this.MaximumCullingDistance = c2;
		}

		// Getters

		[Obsolete]
		public IList<T> GetObjects<T>(Func<GameObject, bool> predicate = null) where T : GameObject
		{
			Dictionary<Type, IList<GameObject>> dict = GetAllObjects(predicate);

			if (!dict.ContainsKey(typeof(T))) return new List<T>();

			return (IList<T>)dict[typeof(T)];

		}

		[Obsolete]
		public Dictionary<Type, IList<GameObject>> GetAllObjects(Func<GameObject, bool> predicate = null)
		{
			return this.ColliderObjects.GetAllObjectsAsTypeDictionary(predicate);
		}

		public Matrix GetViewMatrix()
		{
			Vector3 targetVector =
				this.UseAbsoluteCameraTarget ? this.CameraTarget : this.CameraPosition + 5f * this.CameraTarget;

			// transform camera position because of some black magic
			var viewPosition = new Vector3(this.CameraPosition.X, this.CameraPosition.Z, this.CameraPosition.Y);


			var viewTarget = new Vector3(targetVector.X, targetVector.Z, targetVector.Y);

			//Console.WriteLine($"VM: {viewPosition} {viewTarget}");

			return Matrix.CreateLookAt(viewPosition, viewTarget, Vector3.Up);

		}

		public Matrix GetProjectionMatrix()
		{
			return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, this.MinimumCullingDistance, this.MaximumCullingDistance);
		}

		public abstract void AddLightningToEffect(BasicEffect effect);


		public void Update(GameTime gameTime)
		{

			foreach (GameObject obj in this.ColliderObjects.GetAllObjects()) obj.UpdateLogic(this, gameTime);
			foreach (GameObject obj in ActorObjects) obj.UpdateLogic(this, gameTime);

			// add our buffered objects
			this.ActorObjects.AddRange(this.BufferedActors);
			foreach (GameObject obj in BufferedColliders) this.AddCollider(obj);

			// clear buffers
			this.BufferedActors.Clear();
			this.BufferedColliders.Clear();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (GameObject obj in this.ColliderObjects.GetAllObjects()) obj.DrawLogic(this, spriteBatch);
			foreach (GameObject obj in this.ActorObjects) obj.DrawLogic(this, spriteBatch);
		}

	}
}
