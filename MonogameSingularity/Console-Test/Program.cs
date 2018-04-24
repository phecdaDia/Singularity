using System;
using Microsoft.Xna.Framework;
using Singularity.Code.Utilities;

namespace Console_Test
{
    class Program
    {

		/*
		 * Running tests in this class
		 */
        static void Main(string[] args)
        {
	        RunOctreeTests();
        }

	    private static Boolean RunOctreeTests()
	    {
			// create a new octree
			Octree<Vector3> octree = new Octree<Vector3>(2, -10);



		    return true;
	    }
    }
}
