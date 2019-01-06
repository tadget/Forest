namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Map Settings", menuName = "Map/Map Settings", order = 1)]
    public class MapSettings : ScriptableObject
    {
        [Header("Tile position")]
        public float tileOffsetX = 10f;
        public float tileOffsetZ = 10f;

        [Header("Yard")]
        public int yardSizeX = 6;
        public int yardSizeZ = 4;

        [Header("Outer Layer")]
        public int outerLayerCount = 3;

        [Header("Tile settings")]
        [Header("Fixed")]
        public List<TileSettings> yardTiles;
        public List<TileSettings> outerLayerTiles;

        [Header("Randomized")]
        public List<TileSettings> biome1Tiles;
    }
}
