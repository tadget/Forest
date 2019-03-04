namespace Tadget
{
    using UnityEngine;

    public class Spawn : Actions
    {
        public GameObject objToSpawn;

        /// Spawn objects depending on the field "objToSpawn"
        protected override void Use()
        {
            Instantiate(objToSpawn);
        }
    }
}
