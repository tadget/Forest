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

                    GameObject prefab;
                    if(!tileObjects.TryGetObject(item.id, out prefab))
                        continue;

                    var obj = GameObject.Instantiate(prefab, position, prefab.transform.rotation);
                    if (obj != null)
                        obj.transform.parent = tile_go.transform;
                }
            }

            return tile_go;

            /*
            GameObject tile_prefab;
            if (tile.TryGetTerrain(out tile_prefab))
            {
                tile_go = GameObject.Instantiate(tile_prefab, Vector3.zero, tile_prefab.transform.rotation);
                tile_go.name = string.Format("{0}_{1}", tile.name, tile_go.GetHashCode());
            }
            else
            {
                tile_go = new GameObject();
                tile_go.name = string.Format("{0}_{1}", tile.name, tile_go.GetHashCode());
            }

            int count = tile.isFixedObjectCount ? tile.count : Random.Range(3, 30);
            for (int i = 0; i < count; i++)
            {
                GameObject tree;
                if(tile.TryGetObject(out tree))
                {   
                    float offset_x = tile.isRandomized ? Random.Range(-9f, 9f) : 0;
                    float offset_z = tile.isRandomized ? Random.Range(-9f, 9f) : 0;
                    var obj = GameObject.Instantiate(tree, new Vector3(offset_x, 0f, offset_z), tree.transform.rotation); 

                    //tree.transform.position += new Vector3(offset_x, 0f, offset_z);
                    if(obj != null)
                        obj.transform.parent = tile_go.transform;
                }
            }*/
        }
    }
}
