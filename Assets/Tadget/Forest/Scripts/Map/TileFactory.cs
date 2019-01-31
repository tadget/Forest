namespace Tadget
{
    using UnityEngine;

    public class TileFactory
    {
        public TileFactory()
        {

        }

        public GameObject Create(Tile tile)
        {
            GameObject tile_go;
        if (tile.TryGetTerrain(out tile_go))
            {
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
                    tree.transform.position += new Vector3(offset_x, 0f, offset_z);
                    tree.transform.parent = tile_go.transform;
                }
            }
            return tile_go;
        }
    }
}
