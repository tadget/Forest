using UnityEngine;

namespace Tadget
{
    public class Spawn : Actions
    {

        public GameObject objToSpawn;

        protected override void Use()
        {
            Instantiate(objToSpawn);
        }
    }
}
