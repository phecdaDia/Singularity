using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Singularity.Utilities
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
			CurrentSize = currentSize;
			MinimumSize = minimumSize;
			Precision = precision;

			Max = new Vector3((float) Math.Pow(2, currentSize));
			Min = -Max;

			Center = 0.5f * (Min + Max);

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
			Parent = parent;

			Min = Vector3.Min(corner1, corner2);
			Max = Vector3.Max(corner1, corner2);

			Center = 0.5f * (Min + Max);
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
			if (radius + Precision <= 0.0f)
			{
				// we don't need any axis testing 
				if (CurrentSize > MinimumSize)
				{
					// we are not at the final octree yet
					// create children if they don't exist already.
					var quadrant = GetQuadrantNumber(position);

					if (Children == null)
						Children = new Octree<T>[8];

					if (Children[quadrant] == null)
						Children[quadrant] = new Octree<T>(CurrentSize - 1, MinimumSize, Precision);


					Children[quadrant].AddObject(obj, radius, position);
				}
				else
				{
					// this is the smallest Octree we may create, add as leaf
					//Console.WriteLine($"Adding leaf to octree of size {this.CurrentSize}");
					Leafs.Add(obj);
				}
			}
			else
			{
				// we have to check if we should subpartition it
				if (ShouldSubpartition(position, radius))
				{
					var quadrant = GetQuadrantNumber(position);

					if (Children == null)
						Children = new Octree<T>[8];

					if (Children[quadrant] == null)
						Children[quadrant] = new Octree<T>(CurrentSize - 1, MinimumSize, Precision);

					Children[GetQuadrantNumber(position)].AddObject(obj, radius, position);
				}
				else
				{
					// this quad is small enough for this object
					//Console.WriteLine($"Adding leaf to octree of size {this.CurrentSize}");
					Leafs.Add(obj);
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
			if (obj is IGlobal || !ShouldSubpartition(position, maxScale) || CurrentSize <= MinimumSize)
			{
				//Console.WriteLine($"Adding leaf to octree of size {this.CurrentSize}");
				Leafs.Add(obj);
				return;
			}

			var quadrant = GetQuadrantNumber(position);
			// create children and try again
			//PopulateChildrenNodes();
			if (Children == null)
				Children = new Octree<T>[8];

			if (Children[quadrant] == null)
				Children[quadrant] = new Octree<T>(CurrentSize - 1, MinimumSize, Precision);
			Children[quadrant].AddObject(obj, position, maxScale);
		}

		/// <summary>
		///     Removed an <paramref name="obj" /> at <paramref name="position" />
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool RemoveObject(T obj, Vector3 position)
		{
			if (Leafs.Remove(obj)) return true;

			// it's not in this part, get the children.

			// if there are none however, we have a problem
			if (Children == null) return false;

			var id = GetQuadrantNumber(position);

			if (Children[id] == null) return false;
			return Children[id].RemoveObject(obj, position);
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
			if (RemoveObject(obj, position1))
			{
				AddObject(obj, scale, position2);
			}
			//else
			//{
			//	//Console.WriteLine($"Something went wrong in the octree!");
			//}
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
			if (position.X >= Center.X) quadrant |= 0b100;
			if (position.Y >= Center.Y) quadrant |= 0b010;
			if (position.Z >= Center.Z) quadrant |= 0b001;

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
				Min, // 000
				new Vector3(Min.X, Min.Y, Max.Z), // 001 
				new Vector3(Min.X, Max.Y, Min.Z), // 010
				new Vector3(Min.X, Max.Y, Max.Z), // 011 
				new Vector3(Max.X, Min.Y, Min.Z), // 100 
				new Vector3(Max.X, Min.Y, Max.Z), // 101 
				new Vector3(Max.X, Max.Y, Min.Z), // 110
				Max // 111
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
			if (CurrentSize <= MinimumSize) return false;

			// We use a maximum of 3 control points to see if we should subpartition this tree for a new object
			// first we need to get the intended quadrant.

			var quadrant = GetQuadrantNumber(position);

			// now, depending on the number, we do different tests to see if we should subpartition.

			float x = (quadrant & 0b100) > 0 ? 1 : -1;
			float y = (quadrant & 0b010) > 0 ? 1 : -1;
			float z = (quadrant & 0b001) > 0 ? 1 : -1;

			return quadrant == GetQuadrantNumber(position - (radius + Precision) * new Vector3(x, y, z));
		}

		/// <summary>
		///     Adds children Octrees
		/// </summary>
		private void PopulateChildrenNodes()
		{
			if (Children != null) return; // we already have children.

			//Console.WriteLine($"Creating Children for {{{Min} - {Max}}}");

			Children = new Octree<T>[8]; // 8 children, because it's an OCTree
			var Corners = GetPartitionCorners();

			for (var i = 0; i < 8; i++) Children[i] = new Octree<T>(this, Center, Corners[i]);
		}

		/// <summary>
		///     Removes all leafs and children recursively
		/// </summary>
		public void Clear()
		{
			// cleanup
			if (Children != null)
			{
				for (var i = 0; i < 8; i++)
				{
					if (Children[i] == null)
						continue;
					Children[i].Clear();
					Children[i] = null; // helping the gc a bit
				}

				Children = null;
			}

			Leafs.Clear();
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
			var list = GetAllObjects(predicate);

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
			if (predicate != null) return GetAllObjects().Where(predicate).ToList();

			if (Children == null) return Leafs;

			var output = new List<T>();

			output.AddRange(Leafs.ToArray());

			foreach (var tree in Children)
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
			if (predicate != null) return GetObjects(position).Where(predicate).ToList();

			if (Children == null) return Leafs;
			
			var qn = GetQuadrantNumber(position);

			if (Children[qn] == null) return this.Leafs;


			var output = new List<T>();
			output.AddRange(Leafs.ToArray());
			output.AddRange(Children[qn].GetObjects(position));
			
			return output;
		}
	}
}