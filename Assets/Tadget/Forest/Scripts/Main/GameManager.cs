using UnityEngine.Serialization;

namespace Tadget
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent (typeof(UIManager), typeof(MapManager), typeof(LoadingManager))]
    [RequireComponent((typeof(AudioManager)))]
    public class GameManager : MonoBehaviour
    {   
        /// Map
        private MapManager map;

        /// Player
        private GameObject playerInstance;
        private TileMonitor playerTileMonitor;

        /// UI
        private UIManager ui;

        /// Audio
        private AudioManager sound;

        /// Day Cycle
        private DayCycle day;

        /// Data
        private LoadingManager load;
        [SerializeField]
        private GameData data;
        [SerializeField]
        private GameState state;

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
            LoadGameData();
            state.dataLoaded = true;
            map.Load();
            state.mapLoaded = true;
            PlacePlayer();
            state.playerInstantiated = true;
            LinkPlayerData();
            sound.PlayMainTheme();
            StartCoroutine(GameLoop());
        }

        private void OnApplicationQuit()
        {
            SyncDataFromGame();
            load.SaveGameData(data);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
                OnApplicationQuit();
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
            load = GetComponent<LoadingManager>();
            sound = GetComponent<AudioManager>();
            day = FindObjectOfType<DayCycle>();
            state = GameState.Create();
        }

        private void LoadGameData()
        {
            data = load.GetData();
            SyncGameFromData();
        }

        private void SaveGameData()
        {
            SyncDataFromGame();
            load.SaveGameData(data);
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

        private void SyncGameFromData()
        {
            Debug.Log("Syncing game from data...");
            if (data.isFirstTimePlaying)
            {
                Debug.Log("* Playing for the first time!");
                data.isFirstTimePlaying = false;
            }
            else
            {
                Debug.Log("* Last played " + data.lastPlayedDateTime);
            }

            Debug.Log("* Updating time of day.");
            day.timeOfDay = data.timeOfDay;

            Debug.Log("Data sync complete.");
        }

        private void SyncDataFromGame()
        {
            Debug.Log("Syncing data from game...");

            Debug.Log("* Updating last played time.");
            data.lastPlayedDateTime = DateTime.Now;

            Debug.Log("* Updating time of day.");
            data.timeOfDay = day.timeOfDay;

            Debug.Log("Data sync complete.");
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
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SaveGameData();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                load.ResetData();
                LoadGameData();
            }
        }

        private void UpdatePositionDisplay()
        {
            ui.positionDisplay.text = playerTileMonitor ? 
                playerTileMonitor.tileDisplay : 
                "No positional data";
        }

        private IEnumerator GameLoop()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
