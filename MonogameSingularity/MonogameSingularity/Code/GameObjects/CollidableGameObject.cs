using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code.Enum;

namespace Singularity.Code.GameObjects
{
	public abstract class CollidableGameObject : GameObject
	{
		public Vector3 BoundingBoxMin { get; private set; }
		public Vector3 BoundingBoxMax { get; private set; }

		public CollisionMode CollisionMode { get; private set; }

		public CollidableGameObject() : base()
		{
			this.CollisionMode = CollisionMode.BoundingSphere;
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


		public override void Update(GameScene scene, GameTime gameTime)
		{
			throw new NotImplementedException();
		}
	}
}
