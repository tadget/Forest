namespace Tadget.Map
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapRenderer : MonoBehaviour
    {
        private ChunkFactory chunkFactory;
        private IMapStateProvider mapStateProvider;

        private Dictionary<Vector3Int, Chunk> visibleChunks;
        private Dictionary<Vector3Int, Chunk> cachedChunks;
        private List<Vector3Int> chunksUnderConstruction;
        private List<Chunk> chunksToRecycle;

        public MapSettings mapSettings;
        public TileObjects tileObjects;
        public Action<GameObject> OnDestroyChunkWithSavedObjects;

        private readonly Vector3Int[] neighborCoords = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1)
        };

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

        public MapRenderer Init(IMapStateProvider mapStateProvider)
        {
            tileObjects.Init();
            this.mapStateProvider = mapStateProvider;
            visibleChunks = new Dictionary<Vector3Int, Chunk>();
            cachedChunks = new Dictionary<Vector3Int, Chunk>();
            chunksUnderConstruction = new List<Vector3Int>();
            chunkFactory = gameObject.AddComponent<ChunkFactory>().Init(mapSettings, tileObjects);
            return this;
        }

        public IEnumerator Load(Vector3Int startCoord, Action callback)
        {
            isUpdatingRender = true;
            UpdateMapRender(startCoord);
            yield return new WaitWhile(() => isUpdatingRender);
            callback();
        }

        private Vector3Int lastPositionRequest;
        private bool posRequest;
        private void Update()
        {
            if (posRequest)
            {
                UpdateMapRender(lastPositionRequest);
            }
        }

        private bool isUpdatingRender;
        public void UpdateMapRender(Vector3Int chunkCoord)
        {
            if (chunksUnderConstruction.Count > 0)
            {
                lastPositionRequest = chunkCoord;
                posRequest = true;
                return;
            }
            else
            {
                if (posRequest && lastPositionRequest == chunkCoord)
                    posRequest = false;
            }

            isUpdatingRender = true;

            var visibleChunkCoords = new List<Vector3Int>(visibleChunks.Keys);
            foreach (var visibleChunkCoord in visibleChunkCoords)
            {
                var d = visibleChunkCoord.ChebyshevDistance(chunkCoord);
                // Cache nearby chunks
                if (d == mapSettings.chunkRenderDistance + 1)
                {
                    var chunk = visibleChunks[visibleChunkCoord];
                    chunk.Disable();
                    visibleChunks.Remove(chunk.coord);
                    if (cachedChunks.ContainsKey(chunk.coord))
                    {
                        Debug.LogWarningFormat("Unable to cache chunk. {0} already cached. Recycling.", chunk.coord);
                        chunkFactory.Return(chunk, OnDestroyChunkWithSavedObjects);
                    }
                    else
                    {
                        cachedChunks.Add(chunk.coord, chunk);
                    }
                }
            }

            var cachedChunkCoords = new List<Vector3Int>(cachedChunks.Keys);
            foreach (var cachedChunkCoord in cachedChunkCoords)
            {
                var d = cachedChunkCoord.ChebyshevDistance(chunkCoord);
                // Remove old far away cached chunks
                if (d > mapSettings.chunkRenderDistance + 1)
                {
                    var chunk = cachedChunks[cachedChunkCoord];
                    cachedChunks.Remove(cachedChunkCoord);
                    chunkFactory.Return(chunk, OnDestroyChunkWithSavedObjects);
                }
            }

            foreach (var dir in neighborCoords)
            {
                var targetPos = chunkCoord + dir;
                if(visibleChunks.ContainsKey(targetPos) || chunksUnderConstruction.Contains(targetPos))
                    continue;
                Chunk chunk;
                if (cachedChunks.TryGetValue(targetPos, out chunk))
                {
                    /*Debug.LogFormat("Enabled cached chunk {0}.", targetPos);*/
                    cachedChunks.Remove(targetPos);
                    visibleChunks.Add(targetPos, chunk);
                    chunk.Enable();
                }
                else
                {
                    /*Debug.LogFormat("Requested chunk {0} at frame {1}",
                        targetPos, Time.frameCount);*/
                    chunksUnderConstruction.Add(targetPos);
                    if(mapStateProvider.ShouldRenderHomeAtCoord(targetPos))
                        chunkFactory.Get(targetPos, Chunk.ChunkType.HOME, ChunkCreatedCallback, mapStateProvider.GetSavedHomeChunkObjects());
                    else
                        chunkFactory.Get(targetPos, Chunk.ChunkType.BIOME, ChunkCreatedCallback);
                }
            }
        }

        private void ChunkCreatedCallback(Chunk chunk)
        {
            /*Debug.LogFormat("Received chunk {0} at frame {1}",
                chunk.coord, Time.frameCount);*/

            if (chunksUnderConstruction.Contains(chunk.coord))
            {
                chunksUnderConstruction.Remove(chunk.coord);
                if (chunksUnderConstruction.Count == 0)
                    isUpdatingRender = false;
            }
            else
            {
                Debug.LogWarning("Chunk was not marked for construction");
            }

            if (visibleChunks.ContainsKey(chunk.coord))
            {
                Debug.LogWarningFormat("Created chunk at {0} but another one is already visible at that coordinate. Recycling.",
                    chunk.coord);
                chunkFactory.Return(chunk, OnDestroyChunkWithSavedObjects);
            }
            else
            {
                /*Debug.Log("Created chunk at " + chunk.coord);*/
                visibleChunks.Add(chunk.coord, chunk);
                chunk.Enable();
            }
        }
    }

    public interface IMapStateProvider
    {
        bool ShouldRenderHomeAtCoord(Vector3Int coord);
        GameObject GetSavedHomeChunkObjects();
    }
}
