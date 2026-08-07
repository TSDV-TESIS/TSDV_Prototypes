using Events;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace UI
{
    public class EnemyCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterText;
        [SerializeField] private IntEventChannelSO onEnemyCountUpdate;
        [SerializeField] private VisualEffect counterVFX;
        [SerializeField] private Animator counterAnimator;

        private static readonly int CountEventID = Shader.PropertyToID("Count");


        private static readonly int KillAnimationStateID = Animator.StringToHash("A_EnemyCounter_Kill");

        private int lastEnemyCount = -1;

        private void OnEnable()
        {
            onEnemyCountUpdate.onIntEvent.AddListener(UpdateEnemyCount);
        }

        private void OnDisable()
        {
            onEnemyCountUpdate.onIntEvent.RemoveListener(UpdateEnemyCount);
            lastEnemyCount = -1;
        }

        private void UpdateEnemyCount(int value)
        {
            counterText.text = value.ToString();

            if (lastEnemyCount == -1 || value >= lastEnemyCount)
            {
                lastEnemyCount = value;
                return;
            }
            if (counterVFX != null)
            {
                counterVFX.SendEvent(CountEventID);
            }
            if (counterAnimator != null)
            {

                counterAnimator.PlayInFixedTime(KillAnimationStateID, layer: 0, fixedTime: 0f);
            }

            lastEnemyCount = value;
        }
    }
}