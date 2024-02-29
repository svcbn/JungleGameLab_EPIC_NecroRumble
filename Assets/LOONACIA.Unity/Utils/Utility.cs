using System.Collections;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
    public static class Utility
    {
        public static CoroutineEx Lerp(float from, float to, float duration, System.Action<float> action, System.Action callback = null)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, LerpCoroutine(from, to, duration, action, callback));
        }

        public static CoroutineEx Lerp(Vector3 from, Vector3 to, float duration, System.Action<Vector3> action, System.Action callback = null)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, LerpCoroutine(from, to, duration, action, callback));
        }
        
        public static CoroutineEx EaseLerp(float from, float to, float duration, float curvedness, System.Action<float> action, System.Action callback = null)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, EaseLerpCoroutine(from, to, duration, curvedness, action, callback));
        }
        
        public static CoroutineEx EaseLerp(Vector3 from, Vector3 to, float duration, float curvedness, System.Action<Vector3> action, System.Action callback = null)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, EaseLerpCoroutine(from, to, duration, curvedness, action, callback));
        }


        private static IEnumerator LerpCoroutine(float from, float to, float duration, System.Action<float> action, System.Action callback)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float value = Mathf.Lerp(from, to, time / duration);
                action?.Invoke(value);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }

        private static IEnumerator LerpCoroutine(Vector3 from, Vector3 to, float duration, System.Action<Vector3> action, System.Action callback)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                var value = Vector3.Lerp(from, to, time / duration);
                action?.Invoke(value);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }

        private static IEnumerator EaseLerpCoroutine(float from, float to, float duration, float curvedness, System.Action<float> action, System.Action callback)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float value = EaseLerp(from, to, time / duration, curvedness);
                action?.Invoke(value);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }

        private static IEnumerator EaseLerpCoroutine(Vector3 from, Vector3 to, float duration, float curvedness, System.Action<Vector3> action, System.Action callback)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                var x = EaseLerp(from.x, to.x, time / duration, curvedness);
                var y = EaseLerp(from.y, to.y, time / duration, curvedness);
                var z = EaseLerp(from.z, to.z, time / duration, curvedness);
                var result = new Vector3(x, y, z);
                action?.Invoke(result);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }

        /// <summary>
        /// Interpolates between a and b by t, with curvedness.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation value between two points.</param>
        /// <param name="curvedness">The curvedness of interpolation. 1 means straight line. More than 6 will start to get extreme.</param>
        /// <returns>The curvely interpolated float result between the two float values.</returns>
        private static float EaseLerp(float a, float b, float t, float curvedness)
        {
            return (b-a) * Mathf.Pow(t,curvedness) + a;
        }
    }
}