namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class MapGenerator : MonoBehaviour
    {
        private GameObject mapContainer;
        public MapSettings mapSettings;

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

        public void DestroyMap()
        {
            Destroy(GameObject.Find("Map container"));
        }

        public List<LayoutTile> GenerateMapLayout()
        {
            List<LayoutTile> mapLayout = new List<LayoutTile>();
            mapContainer = new GameObject("Map container");

            /// Generate the yard region (0,0) (yardSizeX,yardSizeZ)
            for (int z = 0; z < mapSettings.yardSizeZ; z++)
                for (int x = 0; x < mapSettings.yardSizeX; x++)
                {
                    // Select tile position
                    Vector3 pos = new Vector3(
                        x * mapSettings.tileOffsetX + x,
                        0,
                        z * mapSettings.tileOffsetZ + z);

                    // Choose tile type
                    MapTile mapTile;
                    if(x == 3 && z == 2)
                    {
                        mapTile = mapSettings.yardTiles[2];
                    }
                    else if(x > 0 && x < mapSettings.yardSizeX - 1 &&
                       z > 0 && z < mapSettings.yardSizeZ - 1)
                    {
                        mapTile = mapSettings.yardTiles[0];
                    }
                    else
                    {
                        mapTile = mapSettings.yardTiles[1];
                    }
                    // Create layout tile and add to map
                    LayoutTile layoutTile = new LayoutTile(mapTile, new Vector3(x, 0, z), pos);
                    mapLayout.Add(layoutTile);
                }

            /// Generate the outer regions with decreasing obstacle density
            for (int layer = 0; layer < mapSettings.outerLayerCount; layer++)
            {
                var layerSizeX = mapSettings.yardSizeX + (layer + 1);
                var layerSizeZ = mapSettings.yardSizeZ + (layer + 1);
                var start = -(layer + 1);
                for (int z = start; z < layerSizeZ; z++)
                    for (int x = start; x < layerSizeX; x++)
                    {
                        if(x > start && x < layerSizeX - 1 &&
                           z > start && z < layerSizeZ - 1)
                           continue;

                        // Select tile position
                        Vector3 pos = new Vector3(
                            x * mapSettings.tileOffsetX + x,
                            0,
                            z * mapSettings.tileOffsetZ + z);

                        // Choose tile type
                        MapTile mapTile;
                        switch (layer)  
                        {
                            case 0:
                            case 1:
                            {
                                mapTile = mapSettings.outerLayerTiles[0];
                                break;
                            }
                            case 2:
                            case 3:
                            {
                                var r = Random.Range(0,2);
                                mapTile = mapSettings.outerLayerTiles[r];
                                break;
                            }
                            case 4:
                            case 5:
                            default:
                            {
                                var r = Random.Range(0,3);
                                mapTile = mapSettings.outerLayerTiles[r];
                                break;
                            }    
                        }
                        // Create layout tile and add to map
                        LayoutTile layoutTile = new LayoutTile(mapTile, new Vector3(x, 0, z), pos);
                        mapLayout.Add(layoutTile);
                    }
            }
            return mapLayout;
        }

        public void LoadMapLayout(List<LayoutTile> mapLayout)
        {
            TileFactory tileFactory = new TileFactory();
            foreach (LayoutTile tile in mapLayout)
            {
                var tile_go = tileFactory.Create(tile);
                var tileID = tile_go.AddComponent<TileID>();
                tileID.gridPosition = tile.gridPosition;
                tile_go.transform.position = tile.worldPosition;
                tile_go.transform.parent = mapContainer.transform;
            }
        }

        public void SaveMapLayout(Dictionary<Vector3, MapTile> mapLayout)
        {

        }

        /// Generate layout or load previous layout
        /// Add cabin
        /// Activate relevant map parts
        /// Save layout
    }
}
