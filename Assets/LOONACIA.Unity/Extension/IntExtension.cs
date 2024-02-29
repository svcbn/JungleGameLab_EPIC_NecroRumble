using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOONACIA.Unity
{
    public static class IntExtension
    {
        public static int ToMask(this int layer)
        {
            return 1 << layer;
        }
    }
}
