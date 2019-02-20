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
            {
                var forestChunk = chunkGenerator.GenerateChunk(position);
                chunks.Add(position, forestChunk);
            }

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
                StartCoroutine(UpdateChunks(tileData.chunk_coord));
            }
        }

        private IEnumerator UpdateChunks(Vector3Int chunkPosition)
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
                            chunk = chunkGenerator.GenerateChunk(targetPos);
                        }
                        chunks.Add(targetPos, chunk);
                    }
                    construction_chunks.Add(targetPos);
                    StartCoroutine(chunkGenerator.InstantiateChunk(InstantiateChunkCallback, chunk, targetPos));
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void InstantiateChunkCallback(Vector3Int pos, GameObject chunk_go)
        {
            if(construction_chunks.Contains(pos))
                construction_chunks.Remove(pos);

            if (active_chunks.ContainsKey(pos))
                active_chunks[pos] = chunk_go;
            else
                active_chunks.Add(pos, chunk_go);
        }

        private void PurgeChunks(Vector3Int coord)
        {
            int count = 0;
            var keys = new List<Vector3Int>(active_chunks.Keys);
            foreach (var key in keys)
            {
                count++;
                if (construction_chunks.Contains(key))
                    continue;
                var d = coord.ManhattanDistance(key);
                if (d >= mapSettings.chunkRenderDistance)
                {
                    var chunk_go = active_chunks[key];
                    StartCoroutine(DelayedDestroy(chunk_go, count));
                    active_chunks.Remove(key);
                    chunks.Remove(key);
                }
            }
        }

        private IEnumerator DelayedDestroy(GameObject go, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
                yield return new WaitForEndOfFrame();
            Destroy(go);
        }

	}
}
