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
        public float volume;

        public override void Use(Action Complete)
        {
            if (source == null)
                source = source.gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            StartCoroutine(PlaySoundAndDestroy(Complete));
        }

        private IEnumerator PlaySoundAndDestroy(Action Complete)
        {
            source.Play();
            yield return new WaitWhile(() => source.isPlaying);
            Complete();
        }
    }
}
