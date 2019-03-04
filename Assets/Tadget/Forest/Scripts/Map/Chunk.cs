namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class Chunk : ScriptableObject
    {
        public Vector3Int coord;
        public GameObject go;
        public Tile[] tiles {get; private set;}
        public int size_x {get; private set;}
        public int size_z {get; private set;}

        public static Chunk Create(Tile[] tiles, int size_x, int size_y, Vector3Int coord)
        {
            return ScriptableObject.CreateInstance<Chunk>().Init(tiles, size_x, size_y, coord);
        }

        private Chunk Init(Tile[] tiles, int size_x, int size_z, Vector3Int coord)
        {
            this.tiles = tiles;
            this.size_x = size_x;
            this.size_z = size_z;
            this.coord = coord;
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

        public void Enable()
        {
            if(go != null)
                go.SetActive(true);
            else
            {
                Debug.LogWarning("Could not enable chunk go.");
            }
        }

        public void Disable()
        {
            if(go != null)
                go.SetActive(false);
            else
            {
                Debug.LogWarning("Could not disable chunk go.");
            }
        }
    }
}


