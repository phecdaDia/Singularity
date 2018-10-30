using System;

namespace Singularity.Utilities
{
	public static class RandomProvider
	{
		public static readonly Random Random = new Random(Environment.TickCount);
	}
}
