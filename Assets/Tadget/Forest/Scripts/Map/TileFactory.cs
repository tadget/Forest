namespace Tadget
{
    using System.Collections.Generic;
    using UnityEngine;

    public class TileFactory
    {
        private TileObjects tileObjects;

        public TileFactory(TileObjects tileObjects)
        {
            this.tileObjects = tileObjects;
        }

        public GameObject Create(Tile tile)
        {
            GameObject tile_go = new GameObject();

            foreach (var item in tile.objects)
            {
                if(item.isRandomChanceToSpawn)
                {
                   if (Random.value > item.chance)
                       continue;
                }

                int count = item.isFixedCount ? item.count : Random.Range(item.minRandomCount, item.maxRandomCount + 1);
                Vector3 position = Vector3.zero;

                GameObject prefab;
                if (!tileObjects.TryGetObject(item.id, out prefab))
                {
                    Debug.LogWarning("Unable to instantiate " + item.id);
                    continue;
                }

                for (int i = 0; i < count; i++)
                {
                    if (item.isPositionRandomized)
                    {
                        float offset_x = Random.Range(item.minOffsetX, item.maxOffsetX);
                        float offset_z = Random.Range(item.minOffsetZ, item.maxOffsetZ);
                        position = new Vector3(offset_x, 0, offset_z);
                    }
                    else
                    {
                        if(item.positions.Count > 0)
                        {
                            position = item.positions[i % item.positions.Count];
                        }
                    }

                    var obj = GameObject.Instantiate(prefab, position, prefab.transform.rotation);
                    if (obj != null)
                        obj.transform.parent = tile_go.transform;
                }
            }

            return tile_go;
        }
    }
}
