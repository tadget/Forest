namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile Settings", menuName = "Map/Tile Settings", order = 1)]
    public class TileSettings : ScriptableObject
    {
        public List<GameObject> objs;
        public List<GameObject> terrains;
        public bool isSpawn;

        public bool TryGetObject(out GameObject obj)
        {
            if (objs != null && objs.Count > 0)
            {
                var p = objs[Random.Range(0, objs.Count)];
                obj = Instantiate(
                    p,
                    Vector3.zero,
                    p.transform.rotation);
                return true;
            }
            else
            {
                obj = null;
                return false;
            }
        }

        public bool TryGetTerrain(out GameObject terrain)
        {
            if (terrains != null && terrains.Count > 0)
            {
                var p = terrains[Random.Range(0, terrains.Count)];
                terrain = Instantiate(
                    p,
                    Vector3.zero,
                   p.transform.rotation);
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
