namespace Tadget
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    [RequireComponent (typeof(MapGenerator))]
    public class GameManager : MonoBehaviour
    {
        public MapGenerator mapGenerator;
        public GameObject player;
        public Vector3 playerSpawnPosition = new Vector3(60f, 0.5f, 20f);
        private GameObject cameraPlayer;
        public Text positionDisplay;
        private TileMonitor tileMonitor;

        public List<LayoutTile> homeLayout;
        private GameObject mapContainer;

        private void Awake()
        {

        }

        private void Start()
        {
            InitVariables();
            LoadMap();
            PlacePlayer();
            LinkPlayerData();
        }

        private void Update()
        {
            CheckForInput();
            UpdatePositionDisplay();
        }

        private void InitVariables()
        {
            mapContainer = new GameObject("Map Container");
            mapGenerator = GetComponent<MapGenerator>();
        }

        private void LoadMap()
        {
            homeLayout = mapGenerator.GenerateHomeLayout();
            var homeTiles = mapGenerator.InstantiateMapLayout(homeLayout);
            GameObject homeTileContainer = new GameObject("Home Tile Container");
            foreach(var tile in homeTiles)
                tile.transform.parent = homeTileContainer.transform;
            homeTileContainer.transform.parent = mapContainer.transform;
        }

        private void PlacePlayer()
        {   
            if(cameraPlayer == null)
                cameraPlayer = Instantiate(player);
            cameraPlayer.transform.position = playerSpawnPosition;
        }

        private void LinkPlayerData()
        {
            tileMonitor = cameraPlayer.GetComponentInChildren<TileMonitor>();
        }

        private void Regenerate()
        {
            foreach(Transform container in mapContainer.transform)
                Destroy(container.gameObject);
            LoadMap();
        }

        private void CheckForInput()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                Regenerate();
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                PlacePlayer();
            }
        }

        private void UpdatePositionDisplay()
        {
            if(tileMonitor)
            {
                positionDisplay.text = tileMonitor.tileDisplay;
            }
            else
            {
                positionDisplay.text = "No positional data";
            }
        }
    }
}