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

            GameObject tree;
            if(tileSettings.TryGetTree(out tree))
            {
                tree.transform.parent = tile.transform;
            }

            return tile;
        }
    }
}