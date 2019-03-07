namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    [CreateAssetMenu(fileName = "Game data", menuName = "Game/Data", order = 1)]
    public class GameData : ScriptableObject
    {
        public bool isFirstTimePlaying = true;
        public bool isLanternFound = false;
        public DateTime lastPlayedDateTime;
        public float timeOfDay;
        public GameObject savedHomeChunkObjects;

        public static GameData Create()
        {
            return ScriptableObject.CreateInstance<GameData>().Init();
        }

        private GameData Init()
        {
            this.isFirstTimePlaying = true;
            this.isLanternFound = false;

            return this;
        }
    }
}
