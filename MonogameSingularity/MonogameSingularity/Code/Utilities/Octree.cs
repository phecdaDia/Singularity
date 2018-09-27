using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Singularity.Core.Utilities
{
	public class Octree<T>
	{
		private Octree<T>[] Children;

		private readonly int CurrentSize;

		private readonly List<T> Leafs = new List<T>();

		private readonly int MinimumSize;

		private Octree<T> Parent;

		private readonly float Precision;

		/// <summary>
		///     Creates a new Octree
		/// </summary>
		/// <param name="currentSize">Size of the Octree</param>
		/// <param name="minimumSize">Smallest partition of the Octree</param>
		/// <param name="precision"></param>
		public Octree(int currentSize, int minimumSize, float precision = 0.0f)
		{
			this.CurrentSize = currentSize;
			this.MinimumSize = minimumSize;
			this.Precision = precision;

			this.Max = new Vector3((float) Math.Pow(2, currentSize));
			this.Min = -this.Max;

			this.Center = 0.5f * (this.Min + this.Max);

			//Console.WriteLine($"Creating Octree of size {this.CurrentSize}");
		}

		/// <summary>
		///     Creates a child Octree
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="corner1"></param>
		/// <param name="corner2"></param>
		private Octree(Octree<T> parent, Vector3 corner1, Vector3 corner2) : this(parent.CurrentSize - 1, parent.MinimumSize,
			parent.Precision)
		{
			this.Parent = parent;

			this.Min = Vector3.Min(corner1, corner2);
			this.Max = Vector3.Max(corner1, corner2);

			this.Center = 0.5f * (this.Min + this.Max);
		}

		// lower border of the octree region
		public Vector3 Min { get; }

		// upper boarder of the octree region
		public Vector3 Max { get; }

		public Vector3 Center { get; }

		/// <summary>
		///     Adds an <paramref name="obj" /> at <paramref name="position" />
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="radius"></param>
		/// <param name="position"></param>
		public void AddObject(T obj, float radius, Vector3 position)
		{
			// we just have a point, therefor we have to create as many octrees as possible
			if (radius + this.Precision <= 0.0f)
			{
				// we don't need any axis testing 
				if (this.CurrentSize > this.MinimumSize)
				{
					// we are not at the final octree yet
					// create children if they don't exist already.
					var quadrant = this.GetQuadrantNumber(position);

					if (this.Children == null)
						this.Children = new Octree<T>[8];

					if (this.Children[quadrant] == null)
						this.Children[quadrant] = new Octree<T>(this, this.Center, this.GetPartitionCorners()[quadrant]);


					this.Children[quadrant].AddObject(obj, radius, position);
				}
				else
				{
					// this is the smallest Octree we may create, add as leaf
					//Console.WriteLine($"Adding leaf to octree of size {this.CurrentSize}");
					this.Leafs.Add(obj);
				}
			}
			else
			{
				// we have to check if we should subpartition it
				if (this.ShouldSubpartition(position, radius))
				{
					var quadrant = this.GetQuadrantNumber(position);

					if (this.Children == null)
						this.Children = new Octree<T>[8];

					if (this.Children[quadrant] == null)
						this.Children[quadrant] = new Octree<T>(this, this.Center, this.GetPartitionCorners()[quadrant]);

					this.Children[this.GetQuadrantNumber(position)].AddObject(obj, radius, position);
				}
				else
				{
					// this quad is small enough for this object
					//Console.WriteLine($"Adding leaf to octree of size {this.CurrentSize}");
					this.Leafs.Add(obj);
				}
			}
		}

		/// <summary>
		///     Adds an <paramref name="obj" /> at <paramref name="position" />
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		/// <param name="maxScale"></param>
		public void AddObject(T obj, Vector3 position, float maxScale)
		{
			if (obj is IGlobal || !this.ShouldSubpartition(position, maxScale))
			{
				//Console.WriteLine($"Adding leaf to octree of size {this.CurrentSize}");

				//Func<Vector3, Vector3, Boolean> IsLess = (a, b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;

				//if (!IsLess(this.Min, position) || !IsLess(position, this.Max))
				//	Console.WriteLine($"Adding {position} to {this.Min} - {this.Max} (Size: {this.CurrentSize})");

				this.Leafs.Add(obj);
				return;
			}

			var quadrant = this.GetQuadrantNumber(position);
			// create children and try again
			//PopulateChildrenNodes();
			if (this.Children == null)
				this.Children = new Octree<T>[8];

			if (this.Children[quadrant] == null)
				this.Children[quadrant] = new Octree<T>(this, this.Center, this.GetPartitionCorners()[quadrant]);
			this.Children[quadrant].AddObject(obj, position, maxScale);
		}

		/// <summary>
		///     Removed an <paramref name="obj" /> at <paramref name="position" />
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool RemoveObject(T obj, Vector3 position)
		{
			if (this.Leafs.Remove(obj)) return true;

			// it's not in this part, get the children.

			// if there are none however, we have a problem
			if (this.Children == null) return false;

			var id = this.GetQuadrantNumber(position);

			if (this.Children[id] == null) return false;
			return this.Children[id].RemoveObject(obj, position);
		}


		/// <summary>
		///     Moves an <paramref name="obj" /> from <paramref name="position1" /> to <paramref name="position2" />
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="scale"></param>
		/// <param name="position1"></param>
		/// <param name="position2"></param>
		public void MoveObject(T obj, float scale, Vector3 position1, Vector3 position2)
		{
			if (this.RemoveObject(obj, position1))
			{
				this.AddObject(obj, scale, position2);
			}
			else
			{
				Console.WriteLine($"Something went wrong in the octree!");
				// Now try to find the object in the octree manually
				var ot = this.FindObjectInOctrees(obj);
				if (ot == null)
				{
					Console.WriteLine("Couldn't find object in Octree at all.");
				}
				else
				{
					// Describe the OT where it was found in
					Console.WriteLine($"Found Object ({position1}) in OT: {ot.Min} {ot.Max}");
				}
			}
		}

		private Octree<T> FindObjectInOctrees(T obj)
		{
			if (this.Leafs.Contains(obj)) return this;

			if (this.Children == null) return null;

			foreach (var child in this.Children)
			{
				var res = child?.FindObjectInOctrees(obj);
				if (res != null) return res;
			}

			return null;
		}

		/// <summary>
		///     Gets Quadrant number
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private int GetQuadrantNumber(Vector3 position)
		{
			// assuming this objects center

			/*
			 * Format: XYZ, where 1 is if T >= Center_T, T = {x, y, z};
			 */

			var quadrant = 0;
			if (position.X >= this.Center.X) quadrant |= 0b100;
			if (position.Y >= this.Center.Y) quadrant |= 0b010;
			if (position.Z >= this.Center.Z) quadrant |= 0b001;

			return quadrant;
		}
		// decide if we should save these corners, although that probably wouldn't save that much 
		// time and memory

		/// <summary>
		///     Gets all 8 Corners of the current Octree.
		/// </summary>
		/// <returns></returns>
		private Vector3[] GetPartitionCorners()
		{
			return new[]
			{
				this.Min, // 000
				new Vector3(this.Min.X, this.Min.Y, this.Max.Z), // 001 
				new Vector3(this.Min.X, this.Max.Y, this.Min.Z), // 010
				new Vector3(this.Min.X, this.Max.Y, this.Max.Z), // 011 
				new Vector3(this.Max.X, this.Min.Y, this.Min.Z), // 100 
				new Vector3(this.Max.X, this.Min.Y, this.Max.Z), // 101 
				new Vector3(this.Max.X, this.Max.Y, this.Min.Z), // 110
				this.Max // 111
			};
		}

		/// <summary>
		///     Decides if we should subpartition this Octree
		/// </summary>
		/// <param name="position"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		private bool ShouldSubpartition(Vector3 position, float radius)
		{
			if (this.CurrentSize <= this.MinimumSize) return false;

			// We use a maximum of 3 control points to see if we should subpartition this tree for a new object
			// first we need to get the intended quadrant.

			var quadrant = this.GetQuadrantNumber(position);

			// now, depending on the number, we do different tests to see if we should subpartition.

			float x = (quadrant & 0b100) > 0 ? 1 : -1;
			float y = (quadrant & 0b010) > 0 ? 1 : -1;
			float z = (quadrant & 0b001) > 0 ? 1 : -1;

			return quadrant == this.GetQuadrantNumber(position - (radius + this.Precision) * new Vector3(x, y, z));
		}

		/// <summary>
		///     Adds children Octrees
		/// </summary>
		private void PopulateChildrenNodes()
		{
			if (this.Children != null) return; // we already have children.

			//Console.WriteLine($"Creating Children for {{{Min} - {Max}}}");

			this.Children = new Octree<T>[8]; // 8 children, because it's an OCTree
			var Corners = this.GetPartitionCorners();

			for (var i = 0; i < 8; i++) this.Children[i] = new Octree<T>(this, this.Center, Corners[i]);
		}

		/// <summary>
		///     Removes all leafs and children recursively
		/// </summary>
		public void Clear()
		{
			// cleanup
			if (this.Children != null)
			{
				for (var i = 0; i < 8; i++)
				{
					if (this.Children[i] == null)
						continue;
					this.Children[i].Clear();
					this.Children[i] = null; // helping the gc a bit
				}

				this.Children = null;
			}

			this.Leafs.Clear();
		}

		// please don't use this method. It has a horrible complexity.
		// this function only exists for compatibility. 
		/// <summary>
		///     GetAllObjectsAsTypeDictionary
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public Dictionary<Type, IList<T>> GetAllObjectsAsTypeDictionary(Func<T, bool> predicate = null)
		{
			var list = this.GetAllObjects(predicate);

			var output = new Dictionary<Type, IList<T>>();

			foreach (var obj in list)
			{
				if (!output.ContainsKey(obj.GetType()))
				{
					var listType = typeof(List<>).MakeGenericType(obj.GetType());
					var cList = (IList<T>) Activator.CreateInstance(listType);
					output.Add(obj.GetType(), cList);
				}

				output[obj.GetType()].Add(obj);
			}

			return output;
		}

		/// <summary>
		///     Gets all objects that match <paramref name="predicate" />
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public List<T> GetAllObjects(Func<T, bool> predicate = null)
		{
			if (predicate != null) return this.GetAllObjects().Where(predicate).ToList();

			if (this.Children == null) return this.Leafs;

			var output = new List<T>();

			output.AddRange(this.Leafs.ToArray());

			foreach (var tree in this.Children)
			{
				if (tree == null) continue;

				output.AddRange(tree.GetAllObjects());
			}

			return output;
		}

		/// <summary>
		///     Gets all objects that match <paramref name="predicate" /> at <paramref name="position" />
		/// </summary>
		/// <param name="position"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public List<T> GetObjects(Vector3 position, Func<T, bool> predicate = null)
		{
			if (predicate != null) return this.GetObjects(position).Where(predicate).ToList();

			if (this.Children == null) return this.Leafs;
			
			var qn = this.GetQuadrantNumber(position);

			if (this.Children[qn] == null) return this.Leafs;


			var output = new List<T>();
			output.AddRange(this.Leafs.ToArray());
			output.AddRange(this.Children[qn].GetObjects(position));
			
			return output;
		}
	}
}