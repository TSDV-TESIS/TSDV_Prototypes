using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Health;
using Player.Shadow;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement), typeof(HealthPoints))]
    public class ShadowStep : MonoBehaviour
    {
        [Header("Shadow")]
        [SerializeField] private GameObject shadowPrefab;
        [SerializeField] private float spawnRate;
        [SerializeField] private float shadowDecayDelay;

        [Header("Visuals")]
        [SerializeField] private MeshRenderer playerRenderer;
        [SerializeField] private Material shadowMat;

        private List<GameObject> _shadows;
        private Coroutine _spawnShadows;
        private bool _isShadow;
        
        void OnEnable()
        {
            _shadows ??= new List<GameObject>();
        }

        public void InitShadowStepShadows()
        {
            _isShadow = true;
            if (_spawnShadows != null)
                StopCoroutine(_spawnShadows);
            _spawnShadows = StartCoroutine(SpawnShadows());
        }

        public void StopShadows()
        {
            _isShadow = false;
        }

        private IEnumerator SpawnShadows()
        {
            ClearShadows();
            float timer = spawnRate;
            float startTime = Time.time;
            _shadows.Add(Instantiate(shadowPrefab, transform.position, quaternion.identity));

            while (_isShadow)
            {
                timer = Time.time - startTime;
                if (timer > spawnRate)
                {
                    startTime = Time.time;
                    _shadows.Add(Instantiate(shadowPrefab, transform.position, quaternion.identity));
                }

                yield return null;
            }

            yield return CollapseShadows();
        }

        private IEnumerator CollapseShadows()
        {
            for (int i = 0; i < _shadows.Count; i++)
            {
                _shadows[i].GetComponent<ShadowExplosion>().Explode();
                yield return new WaitForSeconds(shadowDecayDelay);
            }

            _shadows.Clear();
        }

        private void ClearShadows()
        {
            for (int i = 0; i < _shadows.Count; i++)
            {
                Destroy(_shadows[i]);
            }

            _shadows.Clear();
        }
    }
}