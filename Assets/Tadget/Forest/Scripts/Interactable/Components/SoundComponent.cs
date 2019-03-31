namespace Tadget
{
    using System;
    using UnityEngine;
    using System.Collections;

    public class SoundComponent : ActionComponent
    {
        private AudioSource source;
        public AudioClip clip;
        [Range(0, 1f)]
        public float volume = 1f;

        public override void Use(Action Complete)
        {
            if (source == null)
                source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = clip;
            source.volume = volume;
            StartCoroutine(PlaySound(Complete));
        }

        private IEnumerator PlaySound(Action Complete)
        {
            source.Play();
            yield return new WaitWhile(() => source.isPlaying);
            Complete();
        }
    }
}
