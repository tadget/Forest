namespace Tadget
{
    using UnityEngine;
    using System.Collections.Generic;

    [RequireComponent(typeof(Collider))]
    public class InteractionActivator : ActionActivator
    {
        public void Interact()
        {
            Activate();
        }
    }
}
