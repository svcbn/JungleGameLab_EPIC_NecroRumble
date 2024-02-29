using UnityEngine;

namespace LOONACIA.Unity
{
    public static class ColorExtension
    {
        public static Color ChangedAlpha(this Color originalColor, float newAlpha)
        {
            return new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
        }
    }
}
