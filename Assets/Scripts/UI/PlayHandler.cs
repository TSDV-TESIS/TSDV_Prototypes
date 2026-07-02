using System;
using Events;
using Events.Scriptables;
using Player;
using UnityEngine;

namespace UI
{
    public class PlayHandler : MonoBehaviour
    {
        [SerializeField] private StringEventChannelSO onLoadSceneHandler;
        [SerializeField] private GameObjectEventChannelSO onNewSelectedObject;
        [SerializeField] private PlayerName player;
        [SerializeField] private CinematicHandler videoPlayer;
        [SerializeField] private string level;
        [SerializeField] private VoidEventChannelSO onCinematicEnd;

        private void OnEnable()
        {
            onCinematicEnd?.onEvent.AddListener(StartLevel);
        }

        private void OnDisable()
        {
            onCinematicEnd?.onEvent.RemoveListener(StartLevel);
        }

        public void OnClick()
        {
            if (player.isInitialized)
            {
                StartLevel();
                return;
            }

            onNewSelectedObject?.RaiseEvent(videoPlayer.gameObject);
            videoPlayer.Play();
        }

        void StartLevel()
        {
            onLoadSceneHandler?.RaiseEvent(level);
        }
    }
}