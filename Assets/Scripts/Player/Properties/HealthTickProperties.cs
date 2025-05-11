using UnityEngine;

namespace Player.Properties
{
    [CreateAssetMenu(fileName = "HealthTickProperties", menuName = "Scriptable Objects/HealthTickProperties")]
    public class HealthTickProperties : ScriptableObject
    {
        public int healthTakenPerTick;
        public float secondsPerTick;
        public bool shouldTick;
    }
}
