namespace Tadget
{
    using UnityEngine;

    public class Destroy : Actions
    {

        public GameObject objToDestroy;

        /// Spawn items depending on the field "objToDestroy"
        protected override void Use()
        {
            Destroy(objToDestroy);
        }
    }
}
