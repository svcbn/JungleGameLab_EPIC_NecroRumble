using UnityEngine;

namespace LOONACIA.Unity
{
    public static class FloatExtension
    {
        /// <summary>
        /// Returns 1 if the value is positive, and -1 if it's negative. Returns zero if it's zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Direction(this float value)
        {
            int direction = 0;
            if (value != 0)
            {
                direction = Mathf.RoundToInt(value / Mathf.Abs(value));
            }

            return direction;
        }
    }
}
