namespace Tadget
{
    using UnityEngine;

    [RequireComponent (typeof(MapGenerator))]
    public class GameManager : MonoBehaviour
    {
        public MapGenerator mapGenerator;
        public GameObject player;

        private void Awake()
        {

        }

        private void Start()
        {
            InitVariables();
            LoadMap();
            PlacePlayer();
        }

        private void Update()
        {
            CheckForInput();
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
            var cam = Instantiate(player);
            cam.transform.position = new Vector3(0f, 0.5f, 0f);
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
        }
    }
}