namespace Tadget
{
    using UnityEngine;
    using System;

    public abstract class ActionComponent : MonoBehaviour
    {
        public abstract void Use(Action Complete);
        public float delay;
    }
}

