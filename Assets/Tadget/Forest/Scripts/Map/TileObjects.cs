namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile Objects", menuName = "Map/Tile Objects", order = 1)]
    public class TileObjects : ScriptableObject
    {
        [System.Serializable]
        public class Object
        {
            public GameObject prefab;
            public string id;
        }

        [SerializeField]
        private List<Object> objects;
        private Dictionary<string, GameObject> objectMap;

        public void Init()
        {
            Random.InitState(42);
            objectMap = new Dictionary<string, GameObject>();
            if (objects == null)
                return;
            foreach (var obj in objects)
            {
                if (objectMap.ContainsKey(obj.id))
                {
                    Debug.LogWarningFormat("Trying to add duplicate id {0} to object map. Ensure all objects have unique id.", obj.id);
                }
                else
                {
                    objectMap.Add(obj.id, obj.prefab);
                }
            }
        }

        public bool TryGetObject(string id, out GameObject go)
        {
            go = null;
            return objectMap.TryGetValue(id, out go);
        }
    }
}