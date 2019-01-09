namespace Tadget
{
    using UnityEngine;

    public class TileFactory
    {
        public TileFactory()
        {

        }

        public GameObject Create(LayoutTile layoutTile)
        {
            GameObject tile;
            if (layoutTile.mapTile.TryGetTerrain(out tile))
            {
                tile.name = string.Format("{0}_{1}", layoutTile.mapTile.name, this.GetHashCode());
            }
            else
            {
                tile = new GameObject(string.Format("{0}_{1}", layoutTile.mapTile.name, this.GetHashCode()));
            }

            int count = layoutTile.mapTile.isFixedObjectCount ? layoutTile.mapTile.count : Random.Range(3, 30);
            for (int i = 0; i < count; i++)
            {
                GameObject tree;
                if(layoutTile.mapTile.TryGetObject(out tree))
                {   
                    float offset_x = layoutTile.mapTile.isRandomized ? Random.Range(-9f, 9f) : 0;
                    float offset_z = layoutTile.mapTile.isRandomized ? Random.Range(-9f, 9f) : 0;
                    tree.transform.position += new Vector3(offset_x, 0f, offset_z);
                    tree.transform.parent = tile.transform;
                }
            }
            return tile;
        }
    }
}