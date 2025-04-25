using UnityEngine;

namespace Enemy.Properties
{
    [CreateAssetMenu(fileName = "EnemyPatrolProperties", menuName = "Scriptable Objects/EnemyPatrolProperties")]
    public class EnemyPatrolProperties : ScriptableObject
    {
        [Tooltip("How much distance to patrol point should the enemy be so it can stop")]
        public float distanceToPatrolPoint = 0.5f;
        
        [Header("Idle seconds")]
        public float minIdleSeconds = 1;
        public float maxIdleSeconds = 4;
    }
}
