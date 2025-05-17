using Events.Scriptables;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private InputHandler input;
        [SerializeField] private StringEventChannelSO onLoadScene;
        
        void Start()
        {
            input.OnRestartScene.AddListener(RestartScene);
        }

        public void RestartScene()
        {
            onLoadScene?.RaiseEvent(SceneManager.GetActiveScene().name);
        }
    }
}
