using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Singularity.Code.Utilities;

namespace UnitTests
{
	[TestClass]
	public class OctreeTest
	{
		[TestMethod]
		public void TestOctree()
		{
			// test values
			Vector3[] TestValues = new Vector3[]
			{
				new Vector3(1, 1, 1),
				new Vector3(1, 2, 0),
				new Vector3(0.25f, 0.33f, -0.25f),
				new Vector3(0.25f, 0.33f, -0.24f),
			};

			// Generate Octree
			Octree<Vector3> octree1 = new Octree<Vector3>(2, -5);

			foreach (Vector3 obj in TestValues)
			{
				// add TestValues to octree
				octree1.AddObject(obj, 0.0f, obj);
			}

			foreach (Vector3 obj in TestValues)
			{
				// add TestValues to octree
				octree1.AddObject(obj, 0.40f, obj);
			}

			octree1.Clear();
		}
		[TestMethod]
		public void TestOctree2()
		{
			// test values
			BoundingSphere[][] TestValues = new BoundingSphere[][]
			{
				new BoundingSphere[]
				{
					new BoundingSphere(new Vector3(1, 1, 1), 0.25f)
				},

				new BoundingSphere[]
				{
					new BoundingSphere(new Vector3(0.1f, 0.1f, 0.11f), 0.05f),
					new BoundingSphere(new Vector3(0.1f, 0.1f, 0.21f), 0.05f)
				},



			};

			// Generate Octree
			Octree<Vector3> octree1 = new Octree<Vector3>(5, -5);

			foreach (BoundingSphere[] obj in TestValues)
			{
				// add TestValues to octree
				octree1.AddObject(obj[0].Center, obj[0].Center, 1.0f, obj);
			}

			octree1.Clear();
		}

		[TestMethod]
		public void TestOctree3()
		{
			// test values
			int[] values = new int[]
			{
				0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55
			};

			// Generate Octree
			Octree<int> octree1 = new Octree<int>(16, -5);

			foreach (var v in values)
			{
				int t = v;

				float x = (t % 16) ;
				t /= 16;
				float y = (t % 16) ;
				t /= 16;
				float z = (t % 16);

				octree1.AddObject(v, 0.0f, new Vector3(x, y, z));
			}

			List<int> ret = octree1.GetAllObjects();
			int[] ret2 = ret.ToArray();

			Assert.IsTrue(ret2.Length == values.Length);

			octree1.Clear();

		}
	}
}
