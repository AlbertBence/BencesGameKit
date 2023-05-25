using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bence's Game Kit
namespace BGK.Audio
{
    [CreateAssetMenu(fileName = "New AudioLibrary", menuName = "BGK/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public Audio[] audios;
        public bool AutoGenerateSoundtrack = true;
        public bool AutoGenerateAmbient = true;
        public string[] ManualSoundtrack;
        public string[] ManualAmbient;

        public string[] soundtrack
        {
            get
            {
                if (AutoGenerateSoundtrack)
                {
                    List<string> names = new List<string>();

                    foreach (Audio audio in audios)
                    {
                        if (audio.type == BGK.Audio.AudioType.Music)
                        {
                            names.Add(audio.name);
                        }
                    }

                    return names.ToArray();
                }
                else
                {
                    return ManualSoundtrack;
                }
            }
        }

        public string[] ambient
        {
            get
            {
                if (AutoGenerateAmbient)
                {
                    List<string> names = new List<string>();

                    foreach (Audio audio in audios)
                    {
                        if (audio.type == BGK.Audio.AudioType.Ambient)
                        {
                            names.Add(audio.name);
                        }
                    }

                    return names.ToArray();
                }
                else
                {
                    return ManualAmbient;
                }
            }
        }
    }
}
