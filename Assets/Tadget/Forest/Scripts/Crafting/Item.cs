namespace Tadget
{
    using UnityEngine;

    public class Item : MonoBehaviour
    {
        public int id;
        [Range(0.01f, 2f)]
        public float scaleWhenStored;
    }
}
