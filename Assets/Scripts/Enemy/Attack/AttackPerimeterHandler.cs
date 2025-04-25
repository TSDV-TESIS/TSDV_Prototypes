using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Attack
{
    public class AttackPerimeterHandler : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> onAttackPerimeterTriggered;
    
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                onAttackPerimeterTriggered.Invoke(true);
            }
        }
    
        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                onAttackPerimeterTriggered.Invoke(false);
            }
        }
    }
}
