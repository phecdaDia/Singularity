using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Singularity.Utilities;

namespace UnitTests.Collision
{
	[TestClass]
	public class EquationSystemTest
	{
		[TestMethod]
		public void EquationSystem1()
		{
			// create a plane that spans over the x axis

			// p: x = ZERO3 + (0, 1, 0) + (0, 0, 1)

			// create the plane
			Vector3 planeOrigin     = new Vector3(0, 0, 0);
			Vector3 planeParameter1 = new Vector3(0, 1, 0);
			Vector3 planeParameter2 = new Vector3(0, 0, 1);
			Vector3 planeNormal     = new Vector3(1, 0, 0); // pp1 x pp2

			// create our point of interest
			Vector3 sphere          = new Vector3(-1, 0, 0);

			// We want to solve Ax = b
			// create our Vector "b", which is our solution
			Vector3 b = sphere - planeOrigin;

			// now we can create a room with our plane and the normal which contains all points in R3
			Vector3 solution = VectorMathHelper.SolveLinearEquation(planeParameter1, planeParameter2, planeNormal, b);

			// now we should have a solution. (-1, 0, 0)

		}
	}
}
