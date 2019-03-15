namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LoadingManager : MonoBehaviour
    {
        private ES3File es3File;
        public string filePath = "for.est";

        private void Awake()
        {
            LoadES3File();
        }

        public void SyncToFile()
        {
            Debug.Log("Saving data to file.");
            es3File.Sync();
        }

        private void LoadES3File()
        {
            es3File = new ES3File(System.IO.Path.Combine(Application.persistentDataPath, filePath));
        }

        public GameData GetData(bool loadHomeObjects = true)
        {
            var gameData = GameData.Create();
            if (es3File.KeyExists("gameData"))
            {
                gameData = es3File.Load<GameData>("gameData", gameData);
                if(loadHomeObjects)
                    gameData.savedHomeChunkObjects = LoadGameObject("savedHomeChunkObjects");
            }
            else
            {
                SaveGameData(GameData.Create());
            }

            return gameData;
        }

        public void ResetData()
        {
            var gameData = GameData.Create();
            SaveGameData(gameData);
            Debug.Log("Reset game data!");
        }

        public void SaveGameData(GameData gameData)
        {
            es3File.Save<GameData>("gameData", gameData);
            SaveGameObject(gameData.savedHomeChunkObjects, "savedHomeChunkObjects");
        }

        public void SaveGameObject(GameObject go, string key)
        {
            es3File.Save<GameObject>(key, go);
        }

        public GameObject LoadGameObject(string key)
        {
            if (es3File.KeyExists(key))
                return es3File.Load<GameObject>(key);
            else
                return null;
        }
    }
}
