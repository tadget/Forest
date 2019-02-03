namespace Tadget
{
    using UnityEngine;
    using System.Collections.Generic;

    [DisallowMultipleComponent]
    public class ChunkData : MonoBehaviour
    {
        public List<GameObject> tiles;
        public Vector3Int coord;
    }
}
