using System;
using Core.Systems;
using Core.Patterns;
using UnityEngine;

namespace Core.Hooks {
    public class ParticleReleaseChecker : MonoBehaviour {

        private const float Timer = 0.5f;

        private ParticleSystem _particleSystem;

        private string _particleKey;

        private void Awake() {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable() {
            InvokeRepeating(nameof(CheckRelease), Timer, Timer);
        }

        private void OnDisable() {
            CancelInvoke(nameof(CheckRelease));
        }

        public void SetKey(string particleKey) {
            _particleKey = particleKey;
        }

        private void CheckRelease() {
            if (_particleSystem.IsAlive(true) || _particleSystem.isPlaying) {
                return;
            }
            ParticlePoolSystem.Instance.Release(_particleKey, _particleSystem);
        }
    }
}