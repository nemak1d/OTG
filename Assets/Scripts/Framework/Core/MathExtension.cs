
namespace N1D.Framework.Core
{
	public static class MathExtension
	{
		public const float LocalEpsilon = 0.0001f;

		public static bool IsZero(this float val)
		{
			return -LocalEpsilon < val && val < LocalEpsilon;
		}


		
	}
} // N1D.Framework.Core