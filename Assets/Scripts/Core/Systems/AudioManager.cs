using System;
using DG.Tweening;
using Core.Data;
using Core.Systems;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core.Systems {
    [Serializable]
    public struct AudioPlay {
        public string audioName;
        public string category;
    }

    public static class AudioManager {
        private const int AudioSourceCount = 10;
        private const float MinimumPitch = 0.5f;
        private const float MaximumPitch = 1.2f;
        private const float FadeInVolumeTime = 1.2f;
        private const float FadeOutVolumeTime = 1.4f;

        private static GenericObjectPool<AudioSource> _audioSourcesPool;

        private static GameObject _soundPlayerGameObject;
        private static GameObject _musicPlayerGameObject;
        private static AudioSource _musicAudioSource;

        private static GameObject _ambiancePlayerGameObject;
        private static AudioSource _ambianceAudioSource;

        private static AudioCategoryData[] _categories;

        private static AudioMixer _masterMixer;
        
        /// <summary>
        /// Call this function at the start of the game <b>one time</b>.
        /// </summary>
        public static void InitializeAudioSystem(bool useSound, bool useMusic, bool useAmbiance, AudioCategoryData[] categories, AudioMixer audioMixer) {
            _masterMixer = audioMixer;

            if (useSound) {
                SetSoundVolume(GamePlayerOptions.soundVolume);
                CreateSoundPlayer();
            }
            if (useMusic) {
                SetMusicVolume(GamePlayerOptions.musicVolume);
                CreateMusicPlayer();
            }
            if (useAmbiance) {
                SetAmbianceVolume(GamePlayerOptions.ambianceVolume);
                CreateAmbiancePlayer();
            }

            _categories = categories;
        }

        public static void Play(string category, string audioName) {
            // If there is no sound playing, release them.
            HandleBusyAudioSources();

            // Get the free audio source and play the sound.
            AudioSource readyAudioSource = _audioSourcesPool.Get();
            if (readyAudioSource != null) {
                AudioCategoryData audioCategoryData = GetAudioInfoData(category);
                audioCategoryData.FindAndSetAudio(audioName, readyAudioSource, false);
                readyAudioSource.Play();
            }
        }

        public static void PlayRandomPitch(string category, string audioName, float minPitch=MinimumPitch, float maxPitch=MaximumPitch) {
            // If there is no sound playing, release them.
            HandleBusyAudioSources();

            // Get the free audio source and play the sound.
            AudioSource readyAudioSource = _audioSourcesPool.Get();
            if (readyAudioSource != null) {
                float randomPitch = Random.Range(minPitch, maxPitch);
                AudioCategoryData audioCategoryData = GetAudioInfoData(category);
                audioCategoryData.FindAndSetAudio(audioName, readyAudioSource, false, true, randomPitch);
                readyAudioSource.Play();
            }
        }

        public static void PlayMusic(string category, string audioName) {
            AudioCategoryData audioCategoryData = GetAudioInfoData(category);
            AudioClip audioClip = audioCategoryData.GetClip(audioName);
            if (_musicAudioSource.isPlaying && _musicAudioSource.clip == audioClip) {
                // Already playing that music. No need to restart.
                return;
            }
        
            if (_musicAudioSource.isPlaying) {
                SlowlyFadeOutVolume(_musicAudioSource, () => {
                    audioCategoryData.FindAndSetAudio(audioName, _musicAudioSource, false, looping:true, volumeStartsZero:true);
                    _musicAudioSource.Play();
                    SlowlyFadeInVolume(_musicAudioSource);
                });
            }
            else {
                audioCategoryData.FindAndSetAudio(audioName, _musicAudioSource, false, looping:true, volumeStartsZero:true);
                _musicAudioSource.Play();
                SlowlyFadeInVolume(_musicAudioSource);
            }
        }

        public static void PlayAmbiance(string category, string audioName) {
            AudioCategoryData audioCategoryData = GetAudioInfoData(category);
            AudioClip audioClip = audioCategoryData.GetClip(audioName);
            if (_ambianceAudioSource.isPlaying && _ambianceAudioSource.clip == audioClip) {
                // Already playing that music. No need to restart.
                return;
            }
            if (_ambianceAudioSource.isPlaying) {
                SlowlyFadeOutVolume(_ambianceAudioSource, () => {
                    audioCategoryData.FindAndSetAudio(audioName, _ambianceAudioSource, false, looping:true, volumeStartsZero:true);
                    _ambianceAudioSource.Play();
                    SlowlyFadeInVolume(_ambianceAudioSource);
                });
            }
            else {
                audioCategoryData.FindAndSetAudio(audioName, _ambianceAudioSource, false, looping:true, volumeStartsZero:true);
                _ambianceAudioSource.Play();
                SlowlyFadeInVolume(_ambianceAudioSource);
            }
        }

        public static void StopMusic() {
            _musicAudioSource.Stop();
        }

        /// <summary>
        /// Make sure to set the volume slider value between <b>0.001</b> and <b>1</b>
        /// </summary>
        public static void SetSoundVolume(float volume) {
            _masterMixer.SetFloat("_SoundVolume", Mathf.Log(volume) * 20);
        }
        
        /// <summary>
        /// Make sure to set the volume slider value between <b>0.001</b> and <b>1</b>
        /// </summary>
        public static void SetAmbianceVolume(float volume) {
            _masterMixer.SetFloat("_AmbianceVolume", Mathf.Log(volume) * 20);
        }
        
        /// <summary>
        /// Make sure to set the volume slider value between <b>0.001</b> and <b>1</b>
        /// </summary>
        public static void SetMusicVolume(float volume) {
            _masterMixer.SetFloat("_MusicVolume", Mathf.Log(volume) * 20);
        }

        public static void PauseMusic() {
            _musicAudioSource.Pause();
        }

        public static void ResumeMusic() {
            _musicAudioSource.UnPause();
            SlowlyFadeInVolume(_musicAudioSource);
        }

        public static void StopAmbiance() {
            _ambianceAudioSource.Stop();
        }

        public static void PauseAmbiance() {
            _ambianceAudioSource.Pause();
        }

        public static void ResumeAmbiance() {
            _ambianceAudioSource.UnPause();
            SlowlyFadeInVolume(_ambianceAudioSource);
        }

        private static AudioCategoryData GetAudioInfoData(string category) {
            var categoryData = Array.Find(_categories, t => t.Category == category);

            return categoryData;
        }

        private static void SlowlyFadeInVolume(AudioSource source) {
            // source.do(1, FadeInVolumeTime);
            DOTween.To(() => source.volume, x => source.volume = x, 1, FadeInVolumeTime)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => source.volume = 1); // Ensure volume is set to 1 after fading in
        }

        private static void SlowlyFadeOutVolume(AudioSource source, Action onFinish) {
            // source.DOFade(0, FadeOutVolumeTime).onComplete += () => onFinish?.Invoke();
            DOTween.To(() => source.volume, x => source.volume = x, 0, FadeOutVolumeTime);
        }

        private static void CreateSoundPlayer() {
            _soundPlayerGameObject = new GameObject("SoundPlayer");
            AudioSource[] audioSourceArray = new AudioSource[AudioSourceCount];
            for (int i = 0; i < AudioSourceCount; i++) {
                AudioSource audioSource = _soundPlayerGameObject.AddComponent<AudioSource>();
                audioSourceArray[i] = audioSource;
                _audioSourcesPool = new GenericObjectPool<AudioSource>(AudioSourceCount, audioSourceArray,
                    () => _soundPlayerGameObject.AddComponent<AudioSource>());
            }
            Object.DontDestroyOnLoad(_soundPlayerGameObject);
        }

        private static void CreateMusicPlayer() {
            _musicPlayerGameObject = new GameObject("MusicPlayer");
            _musicAudioSource = _musicPlayerGameObject.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(_musicPlayerGameObject);
        }

        private static void CreateAmbiancePlayer() {
            _ambiancePlayerGameObject = new GameObject("AmbiancePlayer");
            _ambianceAudioSource = _ambiancePlayerGameObject.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(_ambiancePlayerGameObject);
        }

        private static void HandleBusyAudioSources() {
            for (int i = 0; i < _audioSourcesPool.SizeBusyObjects; i++) {
                AudioSource busyAudioSource = _audioSourcesPool.BusyObjects[i];
                if (busyAudioSource.isPlaying == false) {
                    _audioSourcesPool.Release(busyAudioSource);
                }
            }
        }
    }
}