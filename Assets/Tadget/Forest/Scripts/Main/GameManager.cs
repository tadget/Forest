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
            state = GameState.Create();
        }

        private void LoadGameData()
        {
            if (data == null)
            {
                data = load.GetData();
            }
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
            if (Input.GetKeyDown(KeyCode.Z))
            {
                load.SaveGameData(data);
                Debug.Log("Saved game data!");
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                load.ResetData();
                data = load.GetData();
                Debug.Log("Reset game data!");
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
            if (data.isFirstTimePlaying)
            {
                Debug.Log("Playing for the first time!");
                data.isFirstTimePlaying = false;
            }
            else
            {
                Debug.Log("Last played " + data.lastPlayedDateTime);
            }
            data.lastPlayedDateTime = DateTime.Now;
            load.SaveGameData(data);

            while (true)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
