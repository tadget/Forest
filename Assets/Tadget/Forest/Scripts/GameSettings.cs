namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Game settings", menuName = "Game/Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
		[Tooltip("Player (camera) object prefab")]
        public GameObject playerPrefab;
        [Tooltip("Position to spawn player at")]
        public Vector3 playerSpawnPosition = new Vector3(60f, 0.5f, 20f);
    }
}
