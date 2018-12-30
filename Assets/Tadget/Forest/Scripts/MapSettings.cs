namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Map Settings", menuName = "Map/Map Settings", order = 1)]
    public class MapSettings : ScriptableObject
    {
        public int size_x = 10;
        public int size_z = 10; 
        public float tileOffset_x = 10f;
        public float tileOffset_z = 10f;
        public List<TileSettings> tileSettings;
    }
}
