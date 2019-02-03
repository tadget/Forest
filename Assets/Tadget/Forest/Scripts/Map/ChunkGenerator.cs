namespace Tadget
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Random = UnityEngine.Random;

	public class ChunkGenerator
	{
		public const int chunkTileCount_x = 8;
		public const int chunkTileCount_y = 8;

		private MapSettings mapSettings;
        private TileFactory tileFactory;

		public ChunkGenerator(MapSettings mapSettings, TileObjects tileObjects)
		{
			this.mapSettings = mapSettings;
            this.tileFactory = new TileFactory(tileObjects);
		}

        public Chunk GenerateHomeChunk()
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

                    tiles[chunkTileCount_y * z + x] = tile;
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
                        tiles[chunkTileCount_y * z + x] = tile;
                    }
            }
            return Chunk.Create(tiles, 8, 8);
        }

        public Chunk GenerateBiomeChunk(int biome)
        {
	        Tile[] tiles = new Tile[64];
	        for (int i = 0; i < 64; i++)
	        {
                switch (biome)
                {
                    case 0:
                        tiles[i] = mapSettings.biome1Tiles[Random.Range(0, mapSettings.biome1Tiles.Count)];
                        break;
                    case 1:
                        tiles[i] = mapSettings.biome2Tiles[Random.Range(0, mapSettings.biome2Tiles.Count)];
                        break;
                    default:
                        break;
                }
	        }
	        return Chunk.Create(tiles, 8, 8);
        }

        public GameObject InstantiateChunk(Chunk chunk, Vector3Int coord)
        {
	        GameObject chunk_go = new GameObject();
	        chunk_go.name = string.Format("Chunk {0}", chunk.GetInstanceID());

	        List<GameObject> tiles = new List<GameObject>();
	        var start = new Vector3(
		        chunkTileCount_x * mapSettings.tileOffsetX * coord.x,
		        0,
		        chunkTileCount_y * mapSettings.tileOffsetZ * coord.z);
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
			        tileID.chunk_coord = coord;
			        Vector3 pos = new Vector3(
				        x * mapSettings.tileOffsetX,
				        0,
				        z * mapSettings.tileOffsetZ);
			        pos += start;
			        tile_go.transform.position = pos;
			        tile_go.transform.parent = chunk_go.transform;
			        tiles.Add(tile_go);
		        }
	        }

	        var chunkId = chunk_go.AddComponent<ChunkData>();
	        chunkId.tiles = tiles;
	        chunkId.coord = coord;
	        return chunk_go;
        }
	}
}


