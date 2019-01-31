using System.Linq;
using System.Net.NetworkInformation;

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

        private Dictionary<Vector3Int, GameObject> active_chunks;
        private Dictionary<Vector3Int, Chunk> chunks;

        private Vector3Int lastChunkCoord;

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
            chunks = new Dictionary<Vector3Int, Chunk>();
            active_chunks = new Dictionary<Vector3Int, GameObject>();
        }

		public void Load()
        {
            chunks.Clear();
            active_chunks.Clear();

            var homeChunk = chunkGenerator.GenerateHomeChunk();
            chunks.Add(new Vector3Int(0,0,0), homeChunk);

            var forestChunk = chunkGenerator.GenerateBiomeChunk();
            var positions = new List<Vector3Int>()
            {
                new Vector3Int(-1,0,0),
                new Vector3Int(1,0,0),
                new Vector3Int(0,0,-1),
                new Vector3Int(0,0,1),
                new Vector3Int(-1,0,1),
                new Vector3Int(-1,0,-1),
                new Vector3Int(1,0,1),
                new Vector3Int(1,0,-1)
            };

            foreach (var position in positions)
                chunks.Add(position, forestChunk);

            foreach (KeyValuePair<Vector3Int, Chunk> item in chunks)
            {
                var chunk_go = chunkGenerator.InstantiateChunk(item.Value, item.Key);
                active_chunks.Add(item.Key, chunk_go);
                chunk_go.transform.parent = mapContainer.transform;
            }
        }

		public void Regenerate()
        {
            foreach(Transform container in mapContainer.transform)
                Destroy(container.gameObject);
            Load();
        }

        public void OnPlayerEnteredNewTile(TileData tileData)
        {
            Debug.LogFormat("[Map Manager]: Player entered {0} {1} {2} {3}",
                tileData.id, tileData.chunk_coord, tileData.chunk_id, tileData.local_chunk_id);

            if (lastChunkCoord != tileData.chunk_coord)
            {
                Debug.Log("Entered new chunk!");
                lastChunkCoord = tileData.chunk_coord;
                UpdateChunks(tileData.chunk_coord);
                PurgeChunks(tileData.chunk_coord);
            }
        }

        private void UpdateChunks(Vector3Int chunkPosition)
        {
            List<Vector3Int> dirs = new List<Vector3Int>()
            {
                new Vector3Int(-1, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(0, 0, -1),
                new Vector3Int(0, 0, 1),
                new Vector3Int(-1, 0, 1),
                new Vector3Int(-1, 0, -1),
                new Vector3Int(1, 0, 1),
                new Vector3Int(1, 0, -1)
            };
            foreach (var dir in dirs)
            {
                var targetPos = chunkPosition + dir;
                GameObject chunk_go;
                if (active_chunks.TryGetValue(targetPos, out chunk_go))
                {
                    chunk_go.SetActive(true);
                }
                else
                {
                    Chunk chunk;
                    if (chunks.TryGetValue(targetPos, out chunk))
                    {

                    }
                    else
                    {
                        // TODO: choose what chunk to generate
                        chunk = chunkGenerator.GenerateBiomeChunk();
                        chunks.Add(targetPos, chunk);
                    }
                    chunk_go = chunkGenerator.InstantiateChunk(chunk, targetPos);
                    active_chunks.Add(targetPos, chunk_go);
                    chunk_go.transform.parent = mapContainer.transform;
                }
            }
        }

        private void PurgeChunks(Vector3Int chunkPosition)
        {
            int purgeDistance = 3;
            foreach (KeyValuePair<Vector3Int, GameObject> item in active_chunks)
            {
                var d = chunkPosition.ManhattanDistance(item.Key);
                if (d >= purgeDistance)
                {
                    var chunk_go = item.Value;
                    chunk_go.SetActive(false);
                }
            }
        }
	}
}
