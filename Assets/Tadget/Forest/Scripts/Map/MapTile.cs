namespace Tadget
{
    using UnityEngine;

    public class MapTile : ScriptableObject
    {
        public TileSettings tileSettings;

        public MapTile Init(TileSettings tileSettings)
        {
            this.tileSettings = tileSettings;
            return this;
        }

        public GameObject Create()
        {
            var tile = new GameObject(string.Format("{0}_{1}", tileSettings.name, this.GetHashCode()));

            GameObject terrain;
            if (tileSettings.TryGetTerrain(out terrain))
            {
                terrain.transform.parent = tile.transform;
            }

            int count = tileSettings.isSpawn ? 1 : Random.Range(3, 30);
            for (int i = 0; i < count; i++)
            {
                GameObject tree;
                if(tileSettings.TryGetObject(out tree))
                {
                    float offset_x = Random.Range(-9f, 9f);
                    float offset_z = Random.Range(-9f, 9f);
                    tree.transform.position += new Vector3(offset_x, 0f, offset_z);
                    tree.transform.parent = tile.transform;
                }
            }

            return tile;
        }
    }
}