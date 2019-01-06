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

        public Dictionary<Vector3, MapTile> GenerateMapLayout()
        {
            Dictionary<Vector3, MapTile> mapLayout = new Dictionary<Vector3, MapTile>();
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
                    TileSettings tileSettings;
                    if(x > 0 && x < mapSettings.yardSizeX - 1 &&
                       z > 0 && z < mapSettings.yardSizeZ - 1)
                    {
                        tileSettings = mapSettings.yardTiles[0];
                    }
                    else
                    {
                        tileSettings = mapSettings.yardTiles[1];
                    }
                    // Create tile description and add to map
                    MapTile tile;
                    tile = ScriptableObject.CreateInstance<MapTile>().Init(tileSettings);
                    mapLayout.Add(pos, tile);
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
                        TileSettings tileSettings;
                        switch (layer)  
                        {
                            case 0:
                            case 1:
                            {
                                tileSettings = mapSettings.outerLayerTiles[0];
                                break;
                            }
                            case 2:
                            case 3:
                            {
                                var r = Random.Range(0,2);
                                tileSettings = mapSettings.outerLayerTiles[r];
                                break;
                            }
                            case 4:
                            case 5:
                            {
                                var r = Random.Range(0,3);
                                Debug.LogFormat("Layer {0} tile random {1}", layer, r);
                                tileSettings = mapSettings.outerLayerTiles[r];
                                break;
                            }    
                            default:
                                tileSettings = mapSettings.outerLayerTiles[layer];
                                break;
                        }
                        // Create tile description and add to map
                        MapTile tile;
                        tile = ScriptableObject.CreateInstance<MapTile>().Init(tileSettings);
                        mapLayout.Add(pos, tile);
                    }
            }


            return mapLayout;
        }

        public void LoadMapLayout(Dictionary<Vector3, MapTile> mapLayout)
        {
            foreach (KeyValuePair<Vector3, MapTile> tile in mapLayout)
            {
                var tile_go = tile.Value.Create();
                tile_go.transform.position = tile.Key;
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
