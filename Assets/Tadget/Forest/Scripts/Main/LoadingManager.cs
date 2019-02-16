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

        private void OnApplicationQuit()
        {
            es3File.Sync();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationQuit();
        }

        private void LoadES3File()
        {
            es3File = new ES3File(System.IO.Path.Combine(Application.persistentDataPath, filePath));
        }

        public GameData GetData()
        {
            var gameData = GameData.Create();
            if (es3File.KeyExists("gameData"))
            {
                es3File.LoadInto("gameData", gameData);
            }
            else
            {
                SaveGameData(gameData);
            }

            return gameData;
        }

        public void ResetData()
        {
            var gameData = GameData.Create();
            SaveGameData(gameData);
        }

        public void SaveGameData(GameData gameData)
        {
            es3File.Save<GameData>("gameData", gameData);
        }
    }
}
