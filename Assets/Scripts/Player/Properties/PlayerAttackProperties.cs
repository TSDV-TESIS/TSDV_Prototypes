using UnityEngine;

namespace Player.Properties
{
    [CreateAssetMenu(fileName = "Slash Properties", menuName = "Player/Attacks/Slash", order = 0)]
    public class PlayerAttackProperties : ScriptableObject
    {
        [Tooltip("Durations Are displayed In seconds")]
        public float duration = 0.5f;
        public float coolDownDuration = 0.2f;
        public float displacementForce;

        [Tooltip("Damage has to be setted even if it is only one attack")]
        public int damage = 150;
        public float hitStopSeconds = 0.1f;
    }
}