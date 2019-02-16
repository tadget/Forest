namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Game state", menuName = "Game/State", order = 1)]
    public class GameState : ScriptableObject
    {
        public bool dataLoaded;
        public bool mapLoaded;
        public bool playerInstantiated;

        public static GameState Create()
        {
            return Init();
        }

        private static GameState Init()
        {
            return ScriptableObject.CreateInstance<GameState>();
        }
    }
}
