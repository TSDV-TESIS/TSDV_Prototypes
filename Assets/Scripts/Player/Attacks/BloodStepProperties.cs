using UnityEngine;

namespace Player.Attacks
{
    [CreateAssetMenu(fileName = "BloodStepProperties", menuName = "Scriptable Objects/BloodStep Properties")]
    public class BloodStepProperties : ScriptableObject
    {
        public int damage = 150;
        public float hitStopSeconds = 0.2f;
    }
}