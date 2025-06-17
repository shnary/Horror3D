using System;
using System.Collections;
using System.Collections.Generic;
using Core.Hooks;
using UnityEngine;

namespace Core.Systems {
    public class ParticlePoolSystem : MonoBehaviour {
        [Serializable]
        public struct Particle {
            public string key;
            public GameObject prefab;
        }
        
        public static  ParticlePoolSystem Instance { get; private set; }

        private const int PoolSize = 5;

        [SerializeField] private Particle[] particles;

        private Dictionary<string, GenericObjectPool<ParticleSystem>> _particlePoolDictionary;
        
        private void Awake() {
            Instance = this;
            _particlePoolDictionary = new Dictionary<string, GenericObjectPool<ParticleSystem>>();
            foreach (var particle in particles) {
                _particlePoolDictionary[particle.key] = 
                    new GenericObjectPool<ParticleSystem>(PoolSize, 
                        () => CreateParticleSystemForPooling(particle.prefab, particle.key),
                        (p) => p.gameObject.SetActive(false));
            }
        }

        private ParticleSystem CreateParticleSystemForPooling(GameObject prefab, string key) {
            var particleObject = Instantiate(prefab, transform);
            particleObject.SetActive(false);
            var releaseChecker = particleObject.AddComponent<ParticleReleaseChecker>();
            releaseChecker.SetKey(key);
            return particleObject.GetComponent<ParticleSystem>();
        }

        public void Spawn(string particleKey, Vector3 position, Vector3 direction) {
            if (_particlePoolDictionary.ContainsKey(particleKey) == false) {
                Debug.LogError($"{particleKey} could not be found. Please check for typos.");
                return;
            }
            var particle = _particlePoolDictionary[particleKey].Get();
            var particleTransform = particle.transform;
            particleTransform.position = position;
            var eulerAngles = particleTransform.eulerAngles;
            if (direction == Vector3.right) {
                particleTransform.eulerAngles = new Vector3(0, eulerAngles.y, eulerAngles.z);
            }
            else if (direction == Vector3.left) {
                particleTransform.eulerAngles = new Vector3(180, eulerAngles.y, eulerAngles.z);
            }
            else if (direction == Vector3.up) {
                particleTransform.eulerAngles = new Vector3(90, eulerAngles.y, eulerAngles.z);
            }
            else if (direction == Vector3.down) {
                particleTransform.eulerAngles = new Vector3(-90, eulerAngles.y, eulerAngles.z);
            }
            particle.gameObject.SetActive(true);
            particle.Play(true);
        }

        public void Release(string particleKey, ParticleSystem particle) {
            _particlePoolDictionary[particleKey].Release(particle);
        }
    }
}
