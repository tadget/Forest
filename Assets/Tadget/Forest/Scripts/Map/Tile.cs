namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile", menuName = "Map/Tile", order = 1)]
    public class Tile : ScriptableObject
    {
		public Type type;
        
		public enum Type
		{
			UNKNOWN = -1,
			CABIN = 0,
			YARD = 1,
			OUTER = 2,
			BIOME = 3
		}

        [System.Serializable]
        public class Object
        {
            public string id;

            public List<Vector3> positions;
            public bool isPositionRandomized;
            public bool isRotationRandomized;

            public float minOffsetX, maxOffsetX;
            public float minOffsetZ, maxOffsetZ;

            public bool isFixedCount;
            public int count;
            public int minRandomCount, maxRandomCount;

            public bool isRandomChanceToSpawn;
            [Range(0,1)]
            public float chance;
        }

        public List<Object> objects;

    }
}
