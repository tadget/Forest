namespace Tadget.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Map Settings", menuName = "Map/Map Settings", order = 1)]
    public class MapSettings : ScriptableObject
    {
        [Header("Chunk size")]
        public int chunkTileCount_x = 8;
        public int chunkTileCount_z = 8;

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
        public List<Tile> yardTiles;
        public List<Tile> outerLayerTiles;

        [Header("Randomized")]
        public List<Tile> biome1Tiles;
        public List<Tile> biome2Tiles;
        public List<Tile> biome3Tiles;

        public int chunkRenderDistance;
    }
}
