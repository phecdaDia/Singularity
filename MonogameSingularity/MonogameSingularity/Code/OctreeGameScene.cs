//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Singularity.Code.Utilities;

//namespace Singularity.Code
//{
//	public abstract class OctreeGameScene
//	{
//		public Vector3 CameraPosition { get; private set; }     // Current position of the camera
//		public Vector3 CameraTarget { get; private set; }       // Current view direction of the camera
//		private Octree<GameObject> SceneObjects;			   // all current GameObjects in the scene.


//		private IList BufferedSceneObjects;                     // This Dictionary is used when adding new GameObjects to the Scene.
//		public String SceneKey { get; }

//		private Boolean UseAbsoluteCameraTarget = false;



//		public OctreeGameScene(String sceneKey, int sceneSize = 16)
//		{
//			this.SceneKey = sceneKey;

//			// Setting default values for all members
//			this.CameraPosition = new Vector3();
//			//this.SceneObjects = new Dictionary<Type, IList>();
//			this.BufferedSceneObjects = new List<GameObject>();
//		}

//		public void SetupScene()
//		{
//			// clear all current objects.
//			this.SceneObjects.Clear();

//			// Now setup the new objects
//			AddGameObjects();
//		}

//		public void SpawnGameObject(GameObject gameObject)
//		{
//			this.BufferedSceneObjects.Add(gameObject);
//		}

//		protected abstract void AddGameObjects();

//		protected void AddObject(GameObject gameObject)
//		{
//			BoundingSphere[] spheres = new BoundingSphere[gameObject.Model.Meshes.Count];
//			for (int i = 0; i < gameObject.Model.Meshes.Count; i++)
//			{
//				spheres[i] = gameObject.Model.Meshes[i].BoundingSphere;
//			}

//			this.SceneObjects.AddObject(gameObject, gameObject.Position, spheres);
//		}

//		public void SetCamera(Vector3 cameraPosition, Vector3 cameraTarget)
//		{
//			SetCameraPosition(cameraPosition);
//			SetCameraTarget(cameraTarget);
//		}
//		public void SetAbsoluteCamera(Vector3 cameraPosition, Vector3 cameraTarget)
//		{
//			SetCameraPosition(cameraPosition);
//			SetAbsoluteCameraTarget(cameraTarget);
//		}
//		public void SetCameraPosition(Vector3 cameraPosition)
//		{
//			this.CameraPosition = cameraPosition;
//		}

//		public void SetCameraTarget(Vector3 cameraTarget)
//		{
//			this.UseAbsoluteCameraTarget = false;
//			this.CameraTarget = cameraTarget;
//		}

//		public void SetAbsoluteCameraTarget(Vector3 cameraTarget)
//		{
//			this.UseAbsoluteCameraTarget = true;
//			this.CameraTarget = cameraTarget;
//		}

//		// Getters

//		[Obsolete]
//		public IList<T> GetObjects<T>(Func<GameObject, bool> predicate = null) where T : GameObject
//		{
//			Dictionary<Type, IList<GameObject>> dict = GetAllObjects(predicate);

//			if (!dict.ContainsKey(typeof(T))) return new List<T>();

//			return (IList<T>)dict[typeof(T)];

//		}

//		[Obsolete]
//		public Dictionary<Type, IList<GameObject>> GetAllObjects(Func<GameObject, bool> predicate = null)
//		{
//			return this.SceneObjects.GetAllObjectsAsTypeDictionary(predicate);
//		}

//		public Matrix GetViewMatrix()
//		{
//			Vector3 targetVector =
//				this.UseAbsoluteCameraTarget ? this.CameraTarget : this.CameraPosition + 5f * this.CameraTarget;

//			// transform camera position because of some black magic
//			var viewPosition = new Vector3(this.CameraPosition.X, this.CameraPosition.Z, this.CameraPosition.Y);


//			var viewTarget = new Vector3(targetVector.X, targetVector.Z, targetVector.Y);

//			//Console.WriteLine($"VM: {viewPosition} {viewTarget}");

//			return Matrix.CreateLookAt(viewPosition, viewTarget, Vector3.Up);

//		}

//		public Matrix GetProjectionMatrix()
//		{
//			return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 10000f);
//		}

//		public abstract void AddLightningToEffect(BasicEffect effect);


//		public void Update(GameTime gameTime)
//		{
//			foreach (GameObject obj in this.SceneObjects.GetAllObjects())
//			{
//				//obj.UpdateLogic(this, gameTime);
//			}
//		}

//		public void Draw(SpriteBatch spriteBatch)
//		{
//			foreach (GameObject obj in this.SceneObjects.GetAllObjects())
//			{
//				//obj.DrawLogic(this, spriteBatch);
//			}
//		}

//	}
//}
