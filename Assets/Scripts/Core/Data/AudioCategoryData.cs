using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Data {
    [CreateAssetMenu(fileName = "Audio", menuName = "Game/New Audio Category", order = 0)]
    public class AudioCategoryData :ScriptableObject {
        [Serializable]
        private class Audio {
            public string audioName;
            public AudioClip clip;
            public float volume = 1;
            public float pitch = 1;
        }

        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private string categoryName;
        [SerializeField] private Audio[] audios;

        public string Category => categoryName;

        public void FindAndSetAudio(string audioName, AudioSource source, bool play3D, bool useCustomPitch=false, float customPitch=1, bool looping=false, bool volumeStartsZero=false) {
            Audio audio = Array.Find(audios, t => t.audioName.Equals(audioName));
            if (audio == null) {
                return;
            }
            if (string.IsNullOrEmpty(audio.audioName)) {
                Debug.LogError(audioName + " could not be found!");
                return;
            }
            source.clip = audio.clip;
            source.volume = volumeStartsZero ? 0 : audio.volume;
            source.outputAudioMixerGroup = mixerGroup;
            source.pitch = useCustomPitch ? customPitch : audio.pitch;
            source.loop = looping;
            if (play3D) {
                source.maxDistance = 100;
                source.spatialBlend = 1f;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.dopplerLevel = 0f;
            }
            else {
                source.maxDistance = 500;
                source.spatialBlend = 0;
                source.rolloffMode = AudioRolloffMode.Logarithmic;
                source.dopplerLevel = 1f;
            }
        }

        public AudioClip GetClip(string audioName) {
            Audio audio = Array.Find(audios, t => t.audioName.Equals(audioName));
            if (audio == null) {
                Debug.LogError(audioName + " could not be found!");
                return null;
            }
            if (string.IsNullOrEmpty(audio.audioName)) {
                Debug.LogError(audioName + " could not be found!");
                return null;
            }
            return audio.clip;
        }
    }
}