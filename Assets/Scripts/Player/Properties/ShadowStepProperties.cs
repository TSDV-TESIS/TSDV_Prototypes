using UnityEngine;
namespace Player.Properties
{
    [CreateAssetMenu(fileName = "ShadowStep Properties", menuName = "Player/ShadowStep", order = 0)]
    public class ShadowStepProperties : ScriptableObject
    {
        public LayerMask avoidableObjects;
    }
}