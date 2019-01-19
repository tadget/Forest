using System.Linq;

namespace Tadget
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class MapManager : MonoBehaviour 
	{
        private ChunkGenerator chunkGenerator;
		private GameObject mapContainer;
        private GameObject homeTileContainer;
        private GameObject forestTileContainer;

        public MapSettings mapSettings;

        public enum ChunkDir
        {
            MID,
            TOP,
            BOT,
            LEFT,
            RIGHT,
            TOPLEFT,
            TOPRIGHT,
            BOTLEFT,
            BOTRIGHT
        }

        private void OnValidate()
        {
            Debug.AssertFormat(mapSettings != null, "Missing Map Settings");
            if(mapSettings != null)
            {
                Debug.AssertFormat(mapSettings.yardTiles != null &&
                    mapSettings.yardTiles.Count > 1, "Missing Tile Settings inside Map Settings");
                Debug.AssertFormat(mapSettings.outerLayerTiles != null &&
                    mapSettings.outerLayerTiles.Count >= 3, "Missing Tile Settings inside Map Settings");
                Debug.AssertFormat(mapSettings.biome1Tiles != null &&
                    mapSettings.biome1Tiles.Count > 0, "Missing Tile Settings inside Map Settings");
            }
        }

		private void Awake()
		{
			InitVariables();	
		}

        private void Update()
        {

        }
		
		private void InitVariables()
        {
            mapContainer = new GameObject("Map Container");
            chunkGenerator = new ChunkGenerator(mapSettings);
        }

		public void Load()
        {
            var homeChunk = chunkGenerator.GenerateHomeChunk();
            var homeTiles = chunkGenerator.InstantiateChunk(homeChunk);
            homeTileContainer = new GameObject("Home Tile Container");
            foreach(var tile in homeTiles)
                tile.transform.parent = homeTileContainer.transform;
            homeTileContainer.transform.parent = mapContainer.transform;

//            forestTileContainer = new GameObject("Forest Tile Container");
//            forestLayout = mapGenerator.GenerateBiome(new Coord(10, 0, 10), new Coord(10, 0, 1), 0);
//            mapGenerator.InstantiateMapLayout(forestLayout);
        }

		public void Regenerate()
        {
            foreach(Transform container in mapContainer.transform)
                Destroy(container.gameObject);
            Load();
        }

        public void OnPlayerEnteredNewTile(TileID tileID)
        {
            Debug.LogFormat("[Map Manager]: Player entered {0} {1}", tileID.id, tileID.chunk_coord);
//            var n = tileID.coord.GetSurroundingCoords(3).Count;
//            Debug.LogFormat("Got {0} neighbors", n);
//            var ns = string.Join("", tileID.coord.GetSurroundingCoords(3)
//                .ConvertAll(x => x.ToString()).ToArray());
//            Debug.LogFormat("[Map Manager]: Neighbors: {0}", ns);
        }
	}
}
