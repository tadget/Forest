namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ChunkGenerator : MonoBehaviour
    {
        private MapSettings mapSettings;
        private TileFactory tileFactory;

        // private Tile[] tiles;

        public ChunkGenerator Init(MapSettings mapSettings, TileObjects tileObjects)
        {
            this.mapSettings = mapSettings;
            tileFactory = new TileFactory(tileObjects);
            return this;
        }

        private Chunk GenerateHomeChunk(Vector3Int targetPos)
        {
            Tile[] tiles = new Tile[64];

            /// Generate the yard region (3,3) (3+(yardSizeX-1),3+(yardSizeZ-1))
            var origin = new Vector3Int(3, 0, 3);
            for (int z = origin.z; z < origin.z + mapSettings.yardSizeZ; z++)
                for (int x = origin.x; x < origin.x + mapSettings.yardSizeX; x++)
                {
                    Tile tile;
                    if(x == 3 && z == 3)
                    {
                        tile = mapSettings.yardTiles[0];
                        tile.type = Tile.Type.CABIN;
                    }
                    else
                    {
                        tile = mapSettings.yardTiles[1];
                        tile.type = Tile.Type.YARD;
                    }

                    tiles[8 * z + x] = tile;
                }

            /// Generate the outer regions with decreasing obstacle density
            for (int layer = 0; layer < mapSettings.outerLayerCount; layer++)
            {
                var layerSizeX = mapSettings.yardSizeX + (layer + 1) * 2;
                var layerSizeZ = mapSettings.yardSizeZ + (layer + 1) * 2;
                var start = origin.x - (layer + 1);
                for (int z = start; z < start + layerSizeZ; z++)
                    for (int x = start; x < start + layerSizeX; x++)
                    {
                        if(x > start && x < start + layerSizeX - 1 &&
                           z > start && z < start + layerSizeZ - 1)
                           continue;

                        // Choose tile type
                        Tile tile;
                        switch (layer)
                        {
                            case 0:
                            case 1:
                            {
                                tile = mapSettings.outerLayerTiles[0];
                                break;
                            }
                            case 2:
                            case 3:
                            {
                                var r = Random.Range(0,2);
                                tile = mapSettings.outerLayerTiles[r];
                                break;
                            }
                            case 4:
                            case 5:
                            default:
                            {
                                var r = Random.Range(0,3);
                                tile = mapSettings.outerLayerTiles[r];
                                break;
                            }
                        }
                        tile.type = Tile.Type.OUTER;
                        tiles[8 * z + x] = tile;
                    }
            }
            return Chunk.Create(tiles, 8, 8, targetPos);
        }

        private Chunk GenerateChunk(Vector3Int targetPos)
        {
            if (GameManager.state.homeAvailable && targetPos == GameManager.state.homeCoord)
            {
                Debug.Log("Generating home @ " + targetPos);
                return GenerateHomeChunk(targetPos);
            }

            Tile[] tiles = new Tile[mapSettings.chunkTileCount_x * mapSettings.chunkTileCount_y];
            for (int z = 0; z < mapSettings.chunkTileCount_y; z++)
            {
                for (int x = 0; x < mapSettings.chunkTileCount_x; x++)
                {
                    var val = Noise.GenerateNoiseMap(2, 2, Random.Range(0, Int32.MaxValue), 10f, 2, 0.265f, 14,
                        new Vector2(
                            mapSettings.chunkTileCount_x * targetPos.x + x,
                            mapSettings.chunkTileCount_y * targetPos.z + z))[0,0];

                    int tile = z * mapSettings.chunkTileCount_y + x;

                    if(val > 0.8f)
                    {
                        tiles[tile] = mapSettings.biome1Tiles[Random.Range(0, mapSettings.biome1Tiles.Count)];
                    }
                    else if(val > 0.5f)
                    {
                        tiles[tile] = mapSettings.biome2Tiles[Random.Range(0, mapSettings.biome2Tiles.Count)];
                    }
                    else
                    {
                        tiles[tile] = mapSettings.biome3Tiles[Random.Range(0, mapSettings.biome3Tiles.Count)];
                    }
                }
            }
            return Chunk.Create(tiles, mapSettings.chunkTileCount_x, mapSettings.chunkTileCount_y, targetPos);
        }

        public void Get(Vector3Int targetPos, Action<Chunk> callback)
        {
            // Look at tiles that belong to the chunk coordinate
            // Get them and return a logical rendering unit
            Chunk chunk = GenerateChunk(targetPos);
            StartCoroutine(InstantiateChunk(chunk, callback));
        }

        public void Return(Chunk chunk)
        {
            // Decompose chunk
            StartCoroutine(DelayedDestroy(chunk));
            // TODO: use the chunk for something
            // TODO: fix this so it is not in one frame
        }

        bool isDestroying;
        public IEnumerator DelayedDestroy(Chunk chunk)
        {
            if(isDestroying)
                yield return new WaitForEndOfFrame();
            isDestroying = true;
            chunk.Disable();
            yield return new WaitForEndOfFrame();
            Destroy(chunk.go);
            isDestroying = false;
        }

        private bool isInstantiating;
        private IEnumerator InstantiateChunk(Chunk chunk, Action<Chunk> callback)
        {
            if(isInstantiating)
                yield return new WaitForEndOfFrame();
            isInstantiating = true;
            GameObject chunk_go = new GameObject();
            chunk_go.name = string.Format("Chunk {0}", chunk.GetInstanceID());

            List<GameObject> tiles = new List<GameObject>();
            var start = new Vector3(
                mapSettings.chunkTileCount_x * mapSettings.tileOffsetX * chunk.coord.x,
                0,
                mapSettings.chunkTileCount_y * mapSettings.tileOffsetZ * chunk.coord.z);
            var id = -1;
            for (int z = 0; z < chunk.size_z; z++)
            {
                for (int x = 0; x < chunk.size_x; x++)
                {
                    id++;
                    Tile tile = chunk.GetTile(x, z);
                    if (tile == null)
                    {
                        Debug.LogWarningFormat("Null Tile ({0},{1}) in chunk.", x, z);
                        continue;
                    }
                    var tile_go = tileFactory.Create(tile);
                    var tileID = tile_go.AddComponent<TileData>();
                    tileID.id = tile_go.GetInstanceID();
                    tileID.local_chunk_id = id;
                    tileID.chunk_id = chunk.GetInstanceID();
                    tileID.chunk_coord = chunk.coord;
                    Vector3 pos = new Vector3(
                        x * mapSettings.tileOffsetX,
                        0,
                        z * mapSettings.tileOffsetZ);
                    pos += start;
                    tile_go.transform.position = pos;
                    tile_go.transform.parent = chunk_go.transform;
                    tiles.Add(tile_go);
                    if (UnityEngine.Random.value < 0.60f)
                        yield return new WaitForEndOfFrame();
                }
            }

            chunk.go = chunk_go;
            var chunkId = chunk_go.AddComponent<ChunkData>();
            chunkId.tiles = tiles;
            chunkId.coord = chunk.coord;
            yield return new WaitForEndOfFrame();
            callback(chunk);
            isInstantiating = false;
            yield return null;
        }
    }
}


