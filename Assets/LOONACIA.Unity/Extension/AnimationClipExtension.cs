#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimationClipExtension
{
    public static List<Sprite> AllSprites(this AnimationClip clip)
    {
        var sprites = new List<Sprite> ();
        if(clip != null)
        {
            foreach(var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve (clip, binding);
                foreach(var frame in keyframes)
                {
                    sprites.Add((Sprite) frame.value);
                }
            }
        }
        return sprites;
    }
}
#endif