﻿namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapManager : MonoBehaviour
    {
        private ChunkGenerator chunkGenerator;

        private Dictionary<Vector3Int, Chunk> visibleChunks;
        private Dictionary<Vector3Int, Chunk> cachedChunks;
        private List<Vector3Int> chunksUnderConstruction;
        private List<Chunk> chunksToRecycle;

        public MapSettings mapSettings;
        public TileObjects tileObjects;

        private readonly Vector3Int[] neighborCoords = new Vector3Int[]
        {
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, 0),
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

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            tileObjects.Init();
            visibleChunks = new Dictionary<Vector3Int, Chunk>();
            cachedChunks = new Dictionary<Vector3Int, Chunk>();
            chunksUnderConstruction = new List<Vector3Int>();
            chunkGenerator = gameObject.AddComponent<ChunkGenerator>().Init(mapSettings, tileObjects);
        }

        public void Load(Vector3Int homeCoord)
        {
            UpdateMapRender(homeCoord);
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
                        chunkGenerator.Return(chunk);
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
                    chunkGenerator.Return(chunk);
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
                    Debug.LogFormat("Enabled cached chunk {0}.", targetPos);
                    cachedChunks.Remove(targetPos);
                    visibleChunks.Add(targetPos, chunk);
                    chunk.Enable();
                }
                else
                {
                    Debug.LogFormat("Requested chunk {0} at frame {1}",
                        targetPos, Time.frameCount);
                    chunksUnderConstruction.Add(targetPos);
                    chunkGenerator.Get(targetPos, ChunkCreatedCallback);
                }
            }
        }

        private void ChunkCreatedCallback(Chunk chunk)
        {
            Debug.LogFormat("Received chunk {0} at frame {1}",
                chunk.coord, Time.frameCount);

            if (chunksUnderConstruction.Contains(chunk.coord))
            {
                chunksUnderConstruction.Remove(chunk.coord);
            }
            else
            {
                Debug.LogWarning("Chunk was not marked for construction");
            }

            if (visibleChunks.ContainsKey(chunk.coord))
            {
                Debug.LogWarningFormat("Created chunk at {0} but another one is already visible at that coordinate. Recycling.",
                    chunk.coord);
                chunkGenerator.Return(chunk);
            }
            else
            {
                Debug.Log("Created chunk at " + chunk.coord);
                visibleChunks.Add(chunk.coord, chunk);
                chunk.Enable();
            }
        }
    }
}
