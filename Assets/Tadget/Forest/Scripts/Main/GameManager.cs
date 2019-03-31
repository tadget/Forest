namespace Tadget.Main
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using Tadget.Map;

    [RequireComponent (typeof(UIManager), typeof(MapRenderer), typeof(LoadingManager))]
    [RequireComponent(typeof(AudioManager))]
    public class GameManager : MonoBehaviour, IMapStateProvider
    {
        /// Map
        private MapRenderer map;

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
        private GameData data;
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

        private IEnumerator Start()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("main"));
            InitVariables();
            LoadGameData();
            state.SetDataLoaded(true);
            var mapLoadCoroutine = map.Load(Vector3Int.zero, () =>
                state.SetMapLoaded(true));
            StartCoroutine(mapLoadCoroutine);
            yield return new WaitUntil(() => state.mapLoaded);
            PlacePlayer();
            ui.ToggleUICamera(false);
            state.SetPlayerInstantiated(true);
            LinkPlayerData();
            LinkDayCycleEvents();
            LinkGameStateEvents();
            sound.PlayMainTheme();
        }

        private void OnApplicationQuit()
        {
            SyncDataFromGame();
            load.SaveGameData(data);
            load.SyncToFile();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
                OnApplicationQuit();
        }

        private void Update()
        {
            CheckForInput();
            UpdateStateDisplay();
        }

        private void InitVariables()
        {
            ui = GetComponent<UIManager>();
            map = GetComponent<MapRenderer>().Init(this);
            load = GetComponent<LoadingManager>();
            sound = GetComponent<AudioManager>().Init();
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
            EventManager.StartListening<string>("OnHomeAvailable",
                p =>
                {
                    if (state.homeAvailable)
                    {
                        // Debug.Log("Home already available");
                        return;
                    }
                    state.SetHomeAvailable(true);
                    state.SetWasHomeReachedSinceAvailable(false);
                    ChooseNewHomeCoord();
                    Debug.Log(p);
                });

            EventManager.StartListening<TileData>("OnPlayerEnteredNewChunk",
                tileData =>
                {
                    UpdatePlayerStatus(tileData);

                    if (state.homeAvailable)
                    {
                        var d = state.homeCoord.ChebyshevDistance(tileData.chunk.coord);
                        if (state.wasHomeReachedSinceAvailable && d > map.mapSettings.chunkRenderDistance + 1)
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
                state.SetHomeAvailable(true);
                state.SetIsPlayerHome(true);
            }
            else
            {
                Debug.Log("* Last played " + data.lastPlayedDateTime);
                state.SetHomeAvailable(false);
                state.SetIsPlayerHome(false);
            }

            if (data.savedHomeChunkObjects != null)
                Debug.Log("* Loaded saved objects");
            else
            {
                Debug.Log("* No saved objects found");
                data.savedHomeChunkObjects = FindObjectOfType<ES3AutoSave>().gameObject;
            }
            data.savedHomeChunkObjects.transform.position = Vector3.zero;
            data.savedHomeChunkObjects.SetActive(false);

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
            state.playerChunkCoord = tileData.chunk.coord;
            state.SetIsPlayerHome(state.playerChunkCoord == state.homeCoord);
            if(state.homeAvailable && state.isPlayerHome)
                state.SetWasHomeReachedSinceAvailable(true);
        }

        private void UpdateMapCoordData(TileData tileData)
        {
            map.UpdateMapRender(tileData.chunk.coord);
        }

        private void ChooseNewHomeCoord()
        {
            var v = UnityEngine.Random.Range(3, 5);
            var x = UnityEngine.Random.value < .5? 1 : -1;
            var z = UnityEngine.Random.value < .5? 1 : -1;
            state.homeCoord = state.playerChunkCoord + new Vector3Int(x, 0, z) * v;
        }

        public bool ShouldRenderHomeAtCoord(Vector3Int coord)
        {
            return state.homeAvailable && coord == state.homeCoord;
        }

        public GameObject GetSavedObjects()
        {
            return data.savedHomeChunkObjects;
        }
    }
}
