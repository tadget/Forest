namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapGenerator : MonoBehaviour
    {
        public GameObject tilePrefab;
        public GameObject treePrefab;
        public GameObject mapContainer;
        public float tileOffset = 10f;

        [SerializeField]
        public List<List<MapTile>> map;

        void Start()
        {
            GenerateMap();
        }

        void GenerateMap()
        {
            map = new List<List<MapTile>>();
            mapContainer = new GameObject("Map container");

            for (int y = 0; y < 10; y++)
            {
                List<MapTile> row = new List<MapTile>();
                for (int x = 0; x < 10; x++)
                {
                    var pos_x = x * tileOffset + x;
                    var pos_y = y * tileOffset + y;
                    var tile = ScriptableObject.CreateInstance<MapTile>().Init(pos_x, pos_y, tilePrefab, treePrefab);
                    row.Add(tile);
                    var tile_go = tile.Generate();
                    tile_go.transform.parent = mapContainer.transform;
                }
                map.Add(row);
            }
        }

        void Update()
        {

        }
    }

    public class MapTile : ScriptableObject
    {
        public class Pos
        {
            public float x, y;

            public Pos(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public Pos pos;
        public GameObject terrain;
        public GameObject tree;

        public MapTile Init(float x, float y, GameObject terrain, GameObject tree)
        {
            this.pos = new Pos(x, y);
            this.terrain = terrain;
            this.tree = tree;
            return this;
        }

        public GameObject Generate()
        {
            GameObject t = new GameObject("tile");
            if(terrain != null)
            {
                terrain = Instantiate(terrain, new Vector3(pos.x, 0, pos.y), Quaternion.Euler(new Vector3(-90f,0,0)));
                terrain.transform.parent = t.transform;
            }
            if (tree != null)
            {
                tree = Instantiate(tree, new Vector3(pos.x, 0, pos.y), Quaternion.Euler(new Vector3(-90f, 0, 0)));
                tree.transform.parent = t.transform;
            }
            return t;

        }
    }
}

