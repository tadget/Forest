using System;

namespace Tadget
{
    using UnityEngine;
    using System.Collections;

    public static class ExtensionMethods
    {
        public static int ManhattanDistance(this Vector3Int a, Vector3Int b)
        {
            var delta = b - a;
            return Mathf.Abs(delta.x) + Mathf.Abs(delta.y) + Mathf.Abs(delta.z);
        }
    }
}
