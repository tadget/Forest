using UnityEngine;

namespace Tadget
{
    public class Destroy : Actions
    {

        public GameObject objToDestroy;

        protected override void Use()
        {
            Destroy(objToDestroy);
        }
    }
}
