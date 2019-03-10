namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Game state", menuName = "Game/State", order = 1)]
    public class GameState : ScriptableObject
    {
        public bool dataLoaded { private set; get; }
        public bool mapLoaded { private set; get; }
        public bool playerInstantiated { private set; get; }
        public bool homeAvailable { private set; get; }
        public bool isPlayerHome { private set; get; }
        public bool wasHomeReachedSinceAvailable { private set; get; }
        public Vector3Int homeCoord = Vector3Int.zero;
        public Vector3Int playerChunkCoord = Vector3Int.zero;

        public Action OnHomeBecameAvailable;

        public static GameState Create()
        {
            return Init();
        }

        private static GameState Init()
        {
            return ScriptableObject.CreateInstance<GameState>();
        }

        public void SetHomeAvailable(bool available)
        {
            homeAvailable = available;
            if (available)
            {
                if(OnHomeBecameAvailable != null)
                    OnHomeBecameAvailable.Invoke();
            }
        }

        public void SetIsPlayerHome(bool isHome)
        {
            if(isPlayerHome && !isHome)
                Debug.Log("Player left home.");
            isPlayerHome = isHome;
        }

        public void SetWasHomeReachedSinceAvailable(bool reached)
        {
            wasHomeReachedSinceAvailable = reached;
        }

        public void SetDataLoaded(bool loaded)
        {
            dataLoaded = loaded;
        }

        public void SetMapLoaded(bool loaded)
        {
            mapLoaded = loaded;
        }

        public void SetPlayerInstantiated(bool instantiated)
        {
            playerInstantiated = instantiated;
        }
    }
}
