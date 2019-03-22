namespace Tadget.Map
{
    using System.Collections.Generic;
    using UnityEngine;

    public class TileFactory
    {
        /*private TileObjects tileObjects;

        public TileFactory(TileObjects tileObjects)
        {
            this.tileObjects = tileObjects;
        }*/

        public static GameObject Create(Tile tile, Vector3 origin, Transform parent)
        {
            GameObject tile_go = new GameObject();
            tile_go.transform.position = origin;
            tile_go.transform.parent = parent;

            foreach (var item in tile.objects)
            {
                if(item.isRandomChanceToSpawn)
                {
                   if (Random.value > item.chance)
                       continue;
                }

                int count = item.isFixedCount ? item.count : Random.Range(item.minMaxRandomCount.x, item.minMaxRandomCount.y + 1);
                Vector3 position;
                Quaternion rotation;
                GameObject prefab;

                if (item.objectGroup == null || item.objectGroup.Count < 1)
                {
                    Debug.LogWarning("Unable to instantiate item group in tile " + tile.name);
                    continue;
                }

                for (int i = 0; i < count; i++)
                {
                    prefab = item.objectGroup[Random.Range(0, item.objectGroup.Count)].go;
                    if (prefab == null)
                    {
                        Debug.LogWarning("Unable to instantiate prefab in tile " + tile.name);
                        continue;
                    }

                    if (item.isPositionRandomized)
                    {
                        float offset_x = Random.Range(item.minMaxOffset.x, item.minMaxOffset.y);
                        float offset_z = Random.Range(item.minMaxOffset.x, item.minMaxOffset.y);
                        position = new Vector3(origin.x + offset_x, 0, origin.z + offset_z);
                    }
                    else
                    {
                        if(item.positions.Count > 0)
                        {
                            position = origin + item.positions[i % item.positions.Count];
                        }
                        else
                        {
                            position = origin;
                        }
                    }

                    if (item.isRotationRandomized)
                    {
                        rotation = Quaternion.AngleAxis(Random.Range(0,360), Vector3.up) * prefab.transform.rotation;
                    }
                    else
                    {
                        rotation = prefab.transform.rotation;
                    }

                    GameObject.Instantiate(prefab, position, rotation, tile_go.transform);
                }
            }

            return tile_go;
        }
    }
}
