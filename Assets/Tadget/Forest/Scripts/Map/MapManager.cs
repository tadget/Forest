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

        private Dictionary<Vector3Int, GameObject> active_chunks;
        private Dictionary<Vector3Int, Chunk> chunks;
        private List<Vector3Int> construction_chunks;

        private Chunk homeChunk;

        private Vector3Int lastChunkCoord;

        public MapSettings mapSettings;
        public TileObjects tileObjects;

        private void OnValidate()
        {
            Debug.AssertFormat(mapSettings != null, "Missing Map Settings");
            Debug.AssertFormat(tileObjects != null, "Missing Tile Objects");
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
            tileObjects.Init();
            chunkGenerator = new ChunkGenerator(mapSettings, tileObjects);
            chunks = new Dictionary<Vector3Int, Chunk>();
            active_chunks = new Dictionary<Vector3Int, GameObject>();
            construction_chunks = new List<Vector3Int>();
        }

		public void Load()
        {
            chunks.Clear();
            active_chunks.Clear();

            homeChunk = chunkGenerator.GenerateHomeChunk();
            chunks.Add(new Vector3Int(0,0,0), homeChunk);

            var forestChunk = chunkGenerator.GenerateBiomeChunk(0);
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
                StartCoroutine(chunkGenerator.InstantiateChunk(InstantiateChunkCallback, item.Value, item.Key));
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
            //Debug.LogFormat("[Map Manager]: Player entered {0} {1} {2} {3}",
            //    tileData.id, tileData.chunk_coord, tileData.chunk_id, tileData.local_chunk_id);

            if (lastChunkCoord != tileData.chunk_coord)
            {
                //Debug.Log("Entered new chunk!");
                lastChunkCoord = tileData.chunk_coord;
                PurgeChunks(tileData.chunk_coord);
                UpdateChunks(tileData.chunk_coord);
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
                if(construction_chunks.Contains(targetPos))
                    continue;
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
                        if (Random.value > 0.99f)
                            chunk = homeChunk;
                        else
                        {
                            var val = Noise.GenerateNoiseMap(2, 2, 42, 10f, 2, 0.265f, 14, new Vector2(targetPos.x, targetPos.z))[0,0];
                            int biome = 0;
                            if(val > 0.5f)
                            {
                                biome = 0;
                            }
                            else if(val > 0.6f)
                            {
                                biome = 1;
                            }
                            else
                            {
                                biome = 2;
                            }
                            chunk = chunkGenerator.GenerateBiomeChunk(biome);
                        }
                        chunks.Add(targetPos, chunk);
                    }
                    construction_chunks.Add(targetPos);
                    StartCoroutine(chunkGenerator.InstantiateChunk(InstantiateChunkCallback, chunk, targetPos));
                }
            }
        }

        public void InstantiateChunkCallback(Vector3Int pos, GameObject chunk_go)
        {
            if(construction_chunks.Contains(pos))
                construction_chunks.Remove(pos);
            active_chunks.Add(pos, chunk_go);
            chunk_go.transform.parent = mapContainer.transform;
        }

        private void PurgeChunks(Vector3Int coord)
        {
            int purgeDistance = 3;
            //Transform child;
            //List<Transform> tiles = new List<Transform>();
            //List<Transform> objects = new List<Transform>();
            var keys = new List<Vector3Int>(active_chunks.Keys);
            foreach (var key in keys)
            {
                if (construction_chunks.Contains(key))
                    continue;
                var d = coord.ManhattanDistance(key);
                if (d >= purgeDistance)
                {
                    var chunk_go = active_chunks[key];
                    /*
                    foreach(Transform tile in chunk_go.transform)
                    {
                        foreach (Transform obj in tile)
                            Destroy(obj.gameObject);
                        Destroy(tile.gameObject);
                    }*/
                    Destroy(chunk_go);
                    active_chunks.Remove(key);
                    chunks.Remove(key);
                }
            }
        }
	}
}
