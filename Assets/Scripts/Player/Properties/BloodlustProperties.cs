using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [CreateAssetMenu(fileName = "BloodlustProperties", menuName = "Scriptable Objects/Bloodlust Properties")]
    public class BloodlustProperties : ScriptableObject
    {
        public float bloodlustDuration;
        public float bloodlustRecoveryVelocity;
        public float minimumBloodlustPercentageUsage = 0.5f;
    }
}