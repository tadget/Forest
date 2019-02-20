namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class Chunk : ScriptableObject
    {
        public Tile[] tiles {get; private set;}
        public int size_x {get; private set;}
        public int size_z {get; private set;}

        public static Chunk Create(Tile[] tiles, int size_x, int size_y)
        {
            return ScriptableObject.CreateInstance<Chunk>().Init(tiles, size_x, size_y);
        }

        private Chunk Init(Tile[] tiles, int size_x, int size_z)
        {
            this.tiles = tiles;
            this.size_x = size_x;
            this.size_z = size_z;
            return this;
        }

        public Tile GetTile(int x, int z)
        {
            var idx = size_z * z + x;
            if (x >= size_x || idx >= tiles.Length)
            {
                Debug.LogWarningFormat("Trying to access invalid tile x:{0} z:{1}", x, z);
                return null;
            }
            else
            {
                return tiles[idx];
            }
        }

        public Tile GetTile(int idx)
        {
            if (idx >= tiles.Length)
            {
                Debug.LogWarningFormat("Trying to access invalid tile idx:{0}", idx);
                return null;
            }
            else
            {
                return tiles[idx];
            }
        }
    }
}


