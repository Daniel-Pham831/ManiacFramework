using System;
using System.Collections.Generic;
using Maniac.Utils.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Maniac.AudioSystem
{
    [Serializable]
    public class AudioInfo
    {
        [ReadOnly]
        public string name;

        public List<AudioClip> variations = new List<AudioClip>();
        public bool isMusic = false;

        public AudioClip TakeRandom()
        {
            return variations.TakeRandom();
        }
    }
}