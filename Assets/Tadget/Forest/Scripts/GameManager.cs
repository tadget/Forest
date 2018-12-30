namespace Tadget
{
    using UnityEngine;

    [RequireComponent (typeof(MapGenerator))]
    public class GameManager : MonoBehaviour
    {
        public MapGenerator mapGenerator;

        private void Awake()
        {

        }

        private void Start()
        {
            InitVariables();
            LoadMap();
        }

        private void Update()
        {

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
    }
}