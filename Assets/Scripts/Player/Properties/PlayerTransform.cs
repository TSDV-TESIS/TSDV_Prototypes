using UnityEngine;

namespace Player.Properties
{
    [CreateAssetMenu(fileName = "PlayerTransform", menuName = "Scriptable Objects/PlayerTransform")]
    public class PlayerTransform : ScriptableObject
    {
        public Transform playerTransform;
    }
}
