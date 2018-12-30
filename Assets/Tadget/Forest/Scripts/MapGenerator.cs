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
                Debug.AssertFormat(mapSettings.tileSettings != null &&
                    mapSettings.tileSettings.Count > 0, "Missing Tile Settings inside Map Settings");
            }
        }

        public Dictionary<Vector3, MapTile> GenerateMapLayout()
        {
            Dictionary<Vector3, MapTile> mapLayout = new Dictionary<Vector3, MapTile>();
            mapContainer = new GameObject("Map container");

            for (int z = 0; z < mapSettings.size_z; z++)
                for (int x = 0; x < mapSettings.size_x; x++)
                {
                    Vector3 pos = new Vector3(
                        x * mapSettings.tileOffset_x + x,
                        0,
                        z * mapSettings.tileOffset_z + z);

                    var tile = ScriptableObject.CreateInstance<MapTile>().Init(
                        mapSettings.tileSettings[Random.Range(0, mapSettings.tileSettings.Count)]);

                    mapLayout.Add(pos, tile);
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
