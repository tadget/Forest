namespace Tadget.Map
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ChunkFactory : MonoBehaviour
    {
        private MapSettings mapSettings;
        //private TileFactory tileFactory;

        public ChunkFactory Init(MapSettings mapSettings, TileObjects tileObjects)
        {
            this.mapSettings = mapSettings;
            //tileFactory = new TileFactory(tileObjects);
            return this;
        }

        public void Get(Vector3Int targetPos, Chunk.ChunkType chunkType, Action<Chunk> callback)
        {
            Chunk chunk;
            switch (chunkType)
            {
                case Chunk.ChunkType.HOME:
                    chunk = GenerateHomeChunk(targetPos);
                    break;
                case Chunk.ChunkType.BIOME:
                default:
                    chunk = GenerateBiomeChunk(targetPos);
                    break;
            }
            StartCoroutine(InstantiateChunk(chunk, callback));
        }

        public void Return(Chunk chunk)
        {
            StartCoroutine(DestroyChunk(chunk));
        }

        private Chunk GenerateHomeChunk(Vector3Int targetPos)
        {
            Tile[] tiles = new Tile[mapSettings.chunkTileCount_x * mapSettings.chunkTileCount_z];

            for (int z = 0; z < mapSettings.chunkTileCount_z; z++)
            {
                for (int x = 0; x < mapSettings.chunkTileCount_x; x++)
                {
                    int tile = z * mapSettings.chunkTileCount_z + x;
                    if(tile == 6)
                        tiles[tile] = mapSettings.yardTiles[0];
                    else
                        tiles[tile] = mapSettings.yardTiles[1];
                }
            }
            return Chunk.Create(tiles, mapSettings.chunkTileCount_x, mapSettings.chunkTileCount_z, targetPos, Chunk.ChunkType.HOME);
        }

        private Chunk GenerateBiomeChunk(Vector3Int targetPos)
        {
            var noise = Noise.GenerateNoiseMap(mapSettings.chunkTileCount_x, mapSettings.chunkTileCount_z,
                Random.Range(0, Int32.MaxValue),
                10f, 2, 0.265f, 14, Vector2.zero);

            Tile[] tiles = new Tile[mapSettings.chunkTileCount_x * mapSettings.chunkTileCount_z];
            for (int z = 0; z < mapSettings.chunkTileCount_z; z++)
            {
                for (int x = 0; x < mapSettings.chunkTileCount_x; x++)
                {
                    int tile = z * mapSettings.chunkTileCount_z + x;
                    float val = noise[x, z];

                    if(val > 0.8f)
                        tiles[tile] = mapSettings.biome1Tiles[Random.Range(0, mapSettings.biome1Tiles.Count)];
                    else if(val > 0.5f)
                        tiles[tile] = mapSettings.biome2Tiles[Random.Range(0, mapSettings.biome2Tiles.Count)];
                    else
                        tiles[tile] = mapSettings.biome3Tiles[Random.Range(0, mapSettings.biome3Tiles.Count)];
                }
            }
            return Chunk.Create(tiles, mapSettings.chunkTileCount_x, mapSettings.chunkTileCount_z, targetPos, Chunk.ChunkType.BIOME);
        }

        private bool isDestroying;
        private IEnumerator DestroyChunk(Chunk chunk)
        {
            while(isDestroying)
                yield return new WaitForEndOfFrame();
            isDestroying = true;
            chunk.Disable();
            yield return new WaitForEndOfFrame();
            Destroy(chunk.go);
            yield return new WaitForEndOfFrame();
            Destroy(chunk);
            isDestroying = false;
        }

        private bool isInstantiating;
        private IEnumerator InstantiateChunk(Chunk chunk, Action<Chunk> callback)
        {
            while(isInstantiating)
                yield return new WaitForEndOfFrame();
            isInstantiating = true;

            var start = new Vector3(
                mapSettings.chunkTileCount_x * mapSettings.tileOffsetX * chunk.coord.x,
                0,
                mapSettings.chunkTileCount_z * mapSettings.tileOffsetZ * chunk.coord.z);

            GameObject chunk_go = new GameObject();
            chunk_go.name = string.Format("Chunk {0}", chunk.GetInstanceID());
            chunk_go.transform.position = start;
            chunk.go = chunk_go;

            List<GameObject> tiles = new List<GameObject>();

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

                    var tile_go = TileFactory.Create(
                        tile,
                        start + new Vector3(
                            x * mapSettings.tileOffsetX,
                            0,
                            z * mapSettings.tileOffsetZ),
                        chunk_go.transform);

                    tile_go.AddComponent<TileData>().Init(
                        tile_go.GetInstanceID(),
                        id,
                        chunk);

                    tiles.Add(tile_go);

                    if (UnityEngine.Random.value < 0.60f)
                        yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForEndOfFrame();

            callback(chunk);
            isInstantiating = false;
        }
    }
}


