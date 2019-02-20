namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        public AudioData data;
        private AudioSource mainAudioSource;

        private void OnValidate()
        {
            Debug.Assert(data != null, "Missing Audio Data");
        }

        private void Awake()
        {
            var go = new GameObject("Main Audio Source");
            mainAudioSource = go.AddComponent<AudioSource>();
        }

        public void PlayMainTheme()
        {
            mainAudioSource.clip = data.main;
            mainAudioSource.loop = true;
            mainAudioSource.Play();
        }
    }
}
