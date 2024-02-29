using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOONACIA.Unity
{
    public static class Vector2Extension
    {
        /// <summary>
        /// Returns a point in a circle with given center point, radius, and angle. Angle '0' is at 12 o'clock and goes clockwise.
        /// </summary>
        /// <param name="centerPoint">Center point of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="angle">When negative infinity: Random. Starts from 12 o'clock and goes clockwise.</param>
        /// <returns></returns>
        public static Vector2 PointInCircle(this Vector2 centerPoint, float radius, float angle = Mathf.NegativeInfinity)
        {
            if (angle is Mathf.NegativeInfinity) angle = Random.Range(0, 360); //Random angle if not specified.
            
            return centerPoint + (Vector2) (Quaternion.Euler(0,0,-1 * angle) * Vector2.up * radius);
        }
    }
}
