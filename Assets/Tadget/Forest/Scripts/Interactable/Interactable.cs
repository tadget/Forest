namespace Tadget
{
    using UnityEngine;

    [RequireComponent(typeof(Spawn))]
    [RequireComponent(typeof(Destroy))]
    public class Interactable : MonoBehaviour
    {
        // When interacting, an object will be destroyed. Another one will be intiated.
        public void Interact()
        {
            GetComponent<Spawn>().Use();
            GetComponent<Destroy>().Use();
        }
    }
}
