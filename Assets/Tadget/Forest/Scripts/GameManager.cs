namespace Tadget
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent (typeof(MapGenerator))]
    public class GameManager : MonoBehaviour
    {
        public MapGenerator mapGenerator;
        public GameObject player;
        private GameObject cameraPlayer;
        public Text positionDisplay;
        private TileMonitor tileMonitor;

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
            mapGenerator = GetComponent<MapGenerator>();
        }

        private void LoadMap()
        {
            var layout = mapGenerator.GenerateMapLayout();
            mapGenerator.LoadMapLayout(layout);
        }

        private void PlacePlayer()
        {   
            if(cameraPlayer == null)
                cameraPlayer = Instantiate(player);
            cameraPlayer.transform.position = new Vector3(60f, 0.5f, 20f);
        }

        private void LinkPlayerData()
        {
            tileMonitor = cameraPlayer.GetComponentInChildren<TileMonitor>();
        }

        private void Regenerate()
        {
            mapGenerator.DestroyMap();
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