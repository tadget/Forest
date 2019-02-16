namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Audio data", menuName = "Audio/Data", order = 1)]
    public class AudioData : ScriptableObject
    {
        public AudioClip main;
    }
}
