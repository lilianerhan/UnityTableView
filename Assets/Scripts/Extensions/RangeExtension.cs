using UnityEngine.SocialPlatforms;

namespace Assets.Scripts
{
    public static class RangeExtension
    {
        public static int Last(this Range range)
        {
            if (range.count == 0)
            {
                throw new System.InvalidOperationException("Range is invalid");
            }
            return (range.from + range.count - 1);
        }

        public static bool Contains(this Range range, int num)
        {
            return num >= range.from && num < (range.from + range.count);
        }
    }
}