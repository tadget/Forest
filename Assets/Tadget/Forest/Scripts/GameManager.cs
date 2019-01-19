namespace Tadget
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    [RequireComponent (typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {   
        /// Map
        private MapManager map;

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
            map.Load();
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
            ui = GetComponent<UIManager>();
            map = GetComponent<MapManager>();
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
            playerTileMonitor.OnTileEnter += map.OnPlayerEnteredNewTile;
        }

        private void CheckForInput()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                map.Regenerate();
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                PlacePlayer();
            }
        }

        private void UpdatePositionDisplay()
        {
            ui.positionDisplay.text = playerTileMonitor ? 
                playerTileMonitor.tileDisplay : 
                "No positional data";
        }
    }
}