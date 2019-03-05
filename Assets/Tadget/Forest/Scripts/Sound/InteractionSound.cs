namespace Tadget
{
    using UnityEngine;

    public class InteractionSound : MonoBehaviour {

        AudioSource source;

        void Start()
        {
            source = GetComponent<AudioSource>();
            Interactable.Play += PlaySound;
        }

        private void OnDisable()
        {
            Interactable.Play -= PlaySound;
        }

        void PlaySound()
        {
            source.Play();
        }
    }
}
