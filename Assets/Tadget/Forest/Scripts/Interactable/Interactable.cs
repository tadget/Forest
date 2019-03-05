namespace Tadget
{
    using UnityEngine;

    [RequireComponent(typeof(Spawn))]
    [RequireComponent(typeof(Destroy))]
    public class Interactable : MonoBehaviour
    {
        public delegate void PlaySound();
        public static event PlaySound Play;

        // When interacting, an object will be destroyed. Another one will be intiated.
        public void Interact()
        {
            GetComponent<Spawn>().Use();

            if (Play != null)
            {
                Play();
            }

            GetComponent<Destroy>().Use();
        }
    }
}
