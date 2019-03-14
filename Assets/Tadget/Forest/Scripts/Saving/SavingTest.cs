namespace Tadget
{
    using System;
    using Tadget.Map;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SavingTest : MonoBehaviour
    {
        private LoadingManager load;
        public GameObject prefab;
        [SerializeField]
        private KeyCode keyToSave;

        private GameData gameData;

        private void Start()
        {
            load = GetComponent<LoadingManager>();
            gameData = load.GetData();
            if (gameData.savedHomeChunkObjects != null)
                Debug.Log("Loaded saved objects");
            else
            {
                Debug.Log("No objects found");
                gameData.savedHomeChunkObjects = FindObjectOfType<ES3AutoSave>().gameObject;
            }
            gameData.savedHomeChunkObjects.transform.position = Vector3.zero;
            gameData.savedHomeChunkObjects.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyToSave))
            {
                load.SaveGameData(gameData);
                load.SyncToFile();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Instantiate(prefab);
            }
        }
    }
}
