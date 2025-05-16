using System;
using System.Collections.Generic;
using Events;
using Events.Scriptables;
using UnityEngine;

namespace Enemy
{
    public class EnemiesManager : MonoBehaviour
    {
        [Header("Events")] 
        [SerializeField] private GameObjectEventChannelSO onEnemyEnabled;
        [SerializeField] private GameObjectEventChannelSO onEnemyDisabled;
        [SerializeField] private VoidEventChannelSO onAllEnemiesDisabled;

        private List<GameObject> _enemies;

        private void OnEnable()
        {
            _enemies = new List<GameObject>();
            onEnemyEnabled?.onTypedEvent.AddListener(HandleNewEnemy);
            onEnemyDisabled?.onTypedEvent.AddListener(HandleEnemyDisabled);
        }

        private void OnDisable()
        {
            onEnemyEnabled?.onTypedEvent.RemoveListener(HandleNewEnemy);
            onEnemyDisabled?.onTypedEvent.RemoveListener(HandleEnemyDisabled);
        }

        private void HandleEnemyDisabled(GameObject enemy)
        {
            Debug.Log("ENEMY DISABLED!");
            _enemies.Remove(enemy);
            if (_enemies.Count == 0)
            {
                Debug.Log("ALL ENEMIES DISABLED!");
                onAllEnemiesDisabled?.RaiseEvent();
            }
        }

        private void HandleNewEnemy(GameObject enemy)
        {
            Debug.Log("ENEMY ENABLED!");
            _enemies.Add(enemy);
        }
    }
}
