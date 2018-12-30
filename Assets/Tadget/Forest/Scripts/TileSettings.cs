namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile Settings", menuName = "Map/Tile Settings", order = 1)]
    public class TileSettings : ScriptableObject
    {
        public List<GameObject> trees;
        public List<GameObject> terrains;

        public bool TryGetTree(out GameObject tree)
        {
            if (trees != null && trees.Count > 0)
            {
                tree = Instantiate(
                    trees[Random.Range(0, trees.Count)],
                    Vector3.zero,
                    Quaternion.Euler(new Vector3(-90f, 0, 0)));
                return true;
            }
            else
            {
                tree = null;
                return false;
            }
        }

        public bool TryGetTerrain(out GameObject terrain)
        {
            if (terrains != null && terrains.Count > 0)
            {
                terrain = Instantiate(
                    terrains[Random.Range(0, terrains.Count)],
                    Vector3.zero,
                    Quaternion.Euler(new Vector3(-90f, 0, 0)));
                return true;
            }
            else
            {
                terrain = null;
                return false;
            }
        }
    }
}
