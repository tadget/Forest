namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile", menuName = "Map/Tile", order = 1)]
    public class MapTile : ScriptableObject
    {
        public List<GameObject> objs;
        public List<GameObject> terrains;
        public bool isRandomized;
        public bool isFixedObjectCount;
        public int count;

        public bool TryGetObject(out GameObject obj)
        {
            if (objs != null && objs.Count > 0)
            {
                var p = objs[Random.Range(0, objs.Count)];
                obj = Instantiate(
                    p,
                    isRandomized ? new Vector3(0,-0.5f,0f) : Vector3.zero,
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
