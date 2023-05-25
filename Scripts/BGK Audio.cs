using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bence's Game Kit
namespace BGK.Audio
{
    public enum AudioType
    { 
        Music,
        Ambient,
        Effect
    }
    
    [System.Serializable]
    public class Audio
    {
        public string name = "";
        public AudioClip clip = null;
        public float volume = 1f;
        public float pitch = 1f;
        public float space = 0f;
        public bool loop = false;
        public AudioType type = AudioType.Effect;
    }

    public class ManagedAudio
    {
        public AudioSource source;
        public bool PlayBetweenScenes;
        public AudioType type;

        public ManagedAudio(Audio _audio, AudioSource _source, bool _PlayBetweenScenes)
        {
            source = _source;
            source.clip = _audio.clip;
            source.volume = _audio.volume;
            source.pitch = _audio.pitch;
            source.spatialBlend = _audio.space;
            source.loop = _audio.loop;
            type = _audio.type;
            PlayBetweenScenes = _PlayBetweenScenes;
            source.playOnAwake = true;
        }
    }
}
