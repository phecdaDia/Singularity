using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Code
{
	public abstract class GameScene
	{
		private Vector3 CameraPosition;                         // Current position of the camera
		private Vector3 CameraTarget;							// Current view direction of the camera
		private Dictionary<Type, IList> SceneObjects;           // all current GameObjects in the scene.
		private IList BufferedSceneObjects;						// This Dictionary is used when adding new GameObjects to the Scene.
		public String SceneKey { get; }

		public GameScene(String sceneKey)
		{
			this.SceneKey = sceneKey;

			// Setting default values for all members
			this.CameraPosition = new Vector3();
			this.SceneObjects = new Dictionary<Type, IList>();
			this.BufferedSceneObjects = new List<GameObject>();
		}

		public void SetupScene()
		{
			// clear all current objects.
			this.SceneObjects.Clear();

			// Now setup the new objects
			AddGameObjects();
		}

		protected abstract void AddGameObjects();

		protected void AddObject(GameObject gameObject)
		{
			var type = gameObject.GetType();
			if (!SceneObjects.ContainsKey(type))
			{
				var listType = typeof(List<>).MakeGenericType(gameObject.GetType());
				var list = (IList)Activator.CreateInstance(listType);
				SceneObjects.Add(type, list);
			}

			SceneObjects[type].Add(gameObject);
		}

		#region Getter and Setter

		public void SetCamera(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			this.CameraPosition = cameraPosition;
			this.CameraTarget = cameraPosition + cameraTarget;
		}

		// Getters

		public Matrix GetViewMatrix()
		{
			return Matrix.CreateLookAt(this.CameraPosition, this.CameraTarget, Vector3.Up);
		}

		public Matrix GetProjectionMatrix()
		{
			return Matrix.CreatePerspective(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 250f);
		}

		#endregion

		#region Abstract methods

		public abstract void AddLightningToEffect(BasicEffect effect);
		#endregion

		public void Update(GameTime gameTime)
		{
			// now update all objects.
			foreach (var type in SceneObjects.Keys)
			{
				foreach (GameObject obj in SceneObjects[type])
				{
					obj.UpdateLogic(this, gameTime);

					// now update the scripts. 
					foreach (Action<GameScene, GameObject, GameTime> script in obj.GetScripts())
					{
						script.Invoke(this, obj, gameTime);
					}
				}
			}

			// add the scheduled GameObjects
			foreach (GameObject obj in BufferedSceneObjects)
			{
				var type = obj.GetType();
				if (!SceneObjects.ContainsKey(type))
				{
					var listType = typeof(List<>).MakeGenericType(obj.GetType());
					var list = (IList)Activator.CreateInstance(listType);
					SceneObjects.Add(type, list);
				}

				SceneObjects[type].Add(obj);
			}

		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (Type type in SceneObjects.Keys)
			{
				foreach (GameObject obj in SceneObjects[type])
				{
					obj.DrawLogic(this, spriteBatch);
				}

			}
		}

	}
}
