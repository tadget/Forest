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

        public static int ChebyshevDistance(this Vector3Int a, Vector3Int b)
        {
            var dx = Mathf.Abs(a.x - b.x);
            var dy = Mathf.Abs(a.z - b.z);
            const int D = 1;
            const int D2 = 1;
            return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
        }
    }
}
