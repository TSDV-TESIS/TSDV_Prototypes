using UnityEngine;

namespace Objects.Traps
{
    public class TrapContactHandler : MonoBehaviour
    {
        [SerializeField] private BaseTrap baseTrap;

        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player"))
                baseTrap.onTrapContact?.Invoke();
        }
    }
}