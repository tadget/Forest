namespace Tadget
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    [RequireComponent (typeof(MapGenerator), typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {   
        /// Map
        private MapGenerator mapGenerator;

        private GameObject mapContainer;
        private List<LayoutTile> homeLayout;
        private List<LayoutTile> forestLayout;

        /// Player
        private GameObject playerInstance;
        private TileMonitor playerTileMonitor;

        /// UI
        private UIManager ui;

        [Header("Settings")]
        public GameSettings settings;

        private void OnValidate()
        {
            Debug.Assert(settings != null, "Missing Game Settings");
        }

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
            ui = GetComponent<UIManager>();
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
            if(playerInstance == null)
                playerInstance = Instantiate(settings.playerPrefab);
            playerInstance.transform.position = settings.playerSpawnPosition;
        }

        private void LinkPlayerData()
        {
            playerTileMonitor = playerInstance.GetComponentInChildren<TileMonitor>();
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
            if(playerTileMonitor)
            {
                ui.positionDisplay.text = playerTileMonitor.tileDisplay;
            }
            else
            {
                ui.positionDisplay.text = "No positional data";
            }
        }
    }
}