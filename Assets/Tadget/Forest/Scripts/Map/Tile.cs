﻿namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile", menuName = "Map/Tile", order = 1)]
    public class Tile : ScriptableObject
    {
        public List<GameObject> objs;
        public List<GameObject> terrains;
        public bool isRandomized;
        public bool isFixedObjectCount;
        public int count;
		public Type type;
        
		public enum Type
		{
			UNKNOWN = -1,
			CABIN = 0,
			YARD = 1,
			OUTER = 2,
			BIOME = 3
		}

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
