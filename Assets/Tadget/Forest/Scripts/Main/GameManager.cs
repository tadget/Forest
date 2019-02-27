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
        private GameObject homeIndicator;

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
        public static GameState state;

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
            state.SetDataLoaded(true);
            state.SetHomeAvailable(true); //TODO fix to loaded state
            map.Load(state.homeCoord);
            state.SetMapLoaded(true);
            PlacePlayer();
            state.SetPlayerInstantiated(true);
            LinkPlayerData();
            LinkDayCycleEvents();
            LinkGameStateEvents();
            sound.PlayMainTheme();

            // DEBUG TEMPORARY
            homeIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            homeIndicator.transform.localScale *= 10f;
            //
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
            //UpdatePositionDisplay();
            UpdateStateDisplay();
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
            playerTileMonitor.OnChunkEnter += tileData =>
                EventManager.TriggerEvent<TileData>("OnPlayerEnteredNewChunk", tileData);
        }

        private void LinkDayCycleEvents()
        {
            EventManager.StartListening<object>("OnDayTimeEvening",
                p => EventManager.TriggerEvent<string>("OnHomeAvailable", "New home due to evening."));
        }

        private void LinkGameStateEvents()
        {
            /*
            EventManager.StartListening<string>("OnHomeAvailable",
                p =>
                {
                    if (state.homeAvailable)
                    {
                        Debug.Log("Home already available");
                        return;
                    }
                    state.SetHomeAvailable(true);
                    state.SetWasHomeReachedSinceAvailable(false);
                    landmarks.ChooseNewHomeChunkCoord();
                    homeIndicator.transform.position = new Vector3(state.homeCoord.x, 0, state.homeCoord.z) *
                                                       map.mapSettings.tileOffsetX * map.mapSettings.chunkTileCount_x +
                                                       new Vector3(0.5f, 0, 0.5f) * map.mapSettings.tileOffsetX * map.mapSettings.chunkTileCount_x;
                    Debug.Log(p);
                });

            */
            EventManager.StartListening<TileData>("OnPlayerEnteredNewChunk",
                tileData =>
                {
                    UpdatePlayerStatus(tileData);

                    if (state.homeAvailable)
                    {
                        var d = state.homeCoord.ChebyshevDistance(tileData.chunk_coord);
                        if (state.wasHomeReachedSinceAvailable && d > map.mapSettings.chunkRenderDistance)
                        {
                            state.SetHomeAvailable(false);
                            Debug.Log("Home unavailable due to player leaving visible home region.");
                        }
                        else if (d > 4)
                        {
                            state.SetHomeAvailable(false);
                            Debug.Log("Unreached home unavailable due to distance.");
                        }
                    }

                    UpdateMapCoordData(tileData);
                });
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
                playerTileMonitor.tileDisplay + "\n" + state.homeCoord :
                "No positional data";
        }

        private void UpdateStateDisplay()
        {
            ui.stateDisplay.text = string.Format(
                "Player {0}\n" +
                "Home {1}\n" +
                "IsPlayerHome {2}\n" +
                "Home Available {3}\n" +
                "Home Reached {4}",
                state.playerChunkCoord,
                state.homeCoord,
                state.isPlayerHome,
                state.homeAvailable,
                state.wasHomeReachedSinceAvailable);
        }

        private void UpdatePlayerStatus(TileData tileData)
        {
            state.playerChunkCoord = tileData.chunk_coord;
            state.SetIsPlayerHome(state.playerChunkCoord == state.homeCoord);
            if(state.homeAvailable && state.isPlayerHome)
                state.SetWasHomeReachedSinceAvailable(true);
        }

        private void UpdateMapCoordData(TileData tileData)
        {
            map.UpdateMapRender(tileData.chunk_coord);
        }
    }


}
