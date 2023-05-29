using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//Bence's Game Kit
namespace BGK.Audio
{
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        public AudioMixerGroup Music;
        public AudioMixerGroup Ambient;
        public AudioMixerGroup Effect;
        public AudioLibrary AudioLib;
        ManagedAudio[] currentAudios;
        public static AudioManager instance;
        string scene = "";
        int track;
        AudioSource soundtrack;
        int amb;
        AudioSource ambient;
        float pause;
        int nonStop;
        bool initialized;

        // Start is called before the first frame update
        void Start()
        {
            initialized = false;

            if (AudioLib == null)
            {
                Debug.LogError("No AudioLibrary is set.");
                return;
            }

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
                scene = gameObject.scene.name;
                if (currentAudios == null)
                {
                    currentAudios = new ManagedAudio[0];
                }
            }

            DontDestroyOnLoad(gameObject);

            track = 0;
            nonStop = 1;

            if (AudioLib.soundtrack.Length != 0)
            {
                soundtrack = PlayAudio(AudioLib.soundtrack[track], true);
            }

            if (AudioLib.ambient.Length != 0)
            {
                amb = Random.Range(0, AudioLib.ambient.Length);
                ambient = PlayAudio(AudioLib.ambient[amb], true);
            }

            CalcPause();

            initialized = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!initialized)
            {
                Start();
                return;
            }

            for (int i = 0; i < currentAudios.Length; i++)
            {
                if (!currentAudios[i].source.isPlaying)
                {
                    RemoveAudio(i);
                    i--;
                }
            }

            if (AudioLib.soundtrack.Length != 0 && soundtrack == null)
            {
                if (pause <= 0)
                {
                    track++;
                    nonStop++;

                    if (track < AudioLib.soundtrack.Length)
                    {
                        soundtrack = PlayAudio(AudioLib.soundtrack[track], true);
                    }
                    else
                    {
                        track = 0;
                        soundtrack = PlayAudio(AudioLib.soundtrack[track], true);
                    }
                    CalcPause();
                }
                else
                {
                    pause -= Time.deltaTime;
                }
            }

            if (AudioLib.ambient.Length != 0 && ambient == null)
            {
                int NewAmb = amb;

                while (NewAmb == amb && AudioLib.ambient.Length > 1)
                {
                    NewAmb = Random.Range(0, AudioLib.ambient.Length);
                }

                amb = NewAmb;

                ambient = PlayAudio(AudioLib.ambient[amb], true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                PlayAudio("click");
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                PlayAudio("scroll");
            }
        }

        void CalcPause()
        {
            int r = Random.Range(0, AudioLib.soundtrack.Length);
            if (r < nonStop)
            {
                pause = Random.Range(5f, 30f);
                nonStop = 0;
            }
            else
            {
                pause = 0f;
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            if (scene != gameObject.scene.name && scene != "")
            {
                scene = gameObject.scene.name;
                SceneChanged();
            }
            else
            {
                scene = gameObject.scene.name;
            }
        }

        void SceneChanged()
        {
            for (int i = 0; i < currentAudios.Length; i++)
            {
                if (!currentAudios[i].PlayBetweenScenes)
                {
                    RemoveAudio(i);
                    i--;
                }
            }
        }

        public AudioSource PlayAudio(string name, GameObject parent, bool PlayBetweenScenes)
        {
            foreach (Audio a in AudioLib.audios)
            {
                if (a.name == name)
                {
                    return InstanceAudio(a, PlayBetweenScenes, parent);
                }
            }

            Debug.LogWarning("Audio not found: " + name);
            return null;
        }

        public AudioSource PlayAudio(string name)
        {
            return PlayAudio(name, null, false);
        }

        public AudioSource PlayAudio(string name, bool PlayBetweenScenes)
        {
            return PlayAudio(name, null, PlayBetweenScenes);
        }

        public AudioSource PlayAudio(string name, GameObject parent)
        {
            return PlayAudio(name, parent, false);
        }


        AudioSource InstanceAudio(Audio audio, bool PlayBetweenScenes, GameObject parent)
        {
            ManagedAudio[] NewAudios = new ManagedAudio[currentAudios.Length + 1];

            for (int i = 0; i < currentAudios.Length; i++)
            {
                NewAudios[i] = currentAudios[i];
            }

            AudioSource source;

            if (parent == null)
            {
                source = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                source = parent.AddComponent<AudioSource>();
            }

            NewAudios[NewAudios.Length - 1] = new ManagedAudio(audio, source, PlayBetweenScenes);

            if (NewAudios[NewAudios.Length - 1].type == AudioType.Music)
            {
                NewAudios[NewAudios.Length - 1].source.outputAudioMixerGroup = Music;
            }
            else if (NewAudios[NewAudios.Length - 1].type == AudioType.Ambient)
            {
                NewAudios[NewAudios.Length - 1].source.outputAudioMixerGroup = Ambient;
            }
            else
            {
                NewAudios[NewAudios.Length - 1].source.outputAudioMixerGroup = Effect;
            }

            if (parent != null)
            {
                NewAudios[NewAudios.Length - 1].PlayBetweenScenes = false;
            }

            source.Play();

            currentAudios = NewAudios;

            return source;
        }

        public void RemoveAudio(int ind)
        {
            ManagedAudio[] NewAudios = new ManagedAudio[currentAudios.Length - 1];
            for (int i = 0; i < NewAudios.Length; i++)
            {
                if (i < ind)
                {
                    NewAudios[i] = currentAudios[i];
                }
                else
                {
                    NewAudios[i] = currentAudios[i + 1];
                }
            }

            Destroy(currentAudios[ind].source);

            currentAudios = NewAudios;
        }

        public void RemoveAllAudio()
        {
            for (int i = 0; i < currentAudios.Length; i++)
            {
                Destroy(currentAudios[i].source);
            }

            currentAudios = new ManagedAudio[0];
        }

        private void OnDestroy()
        {
            RemoveAllAudio();
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
