using System;
using Events;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class WwiseClockSynchronizer : MonoBehaviour
    {
        [Header("Scriptables")] 
        [SerializeField] private InputHandler handler;

        [Header("Wwise events")]
        [SerializeField] private AK.Wwise.Event playHeartbeatEvent;
        [SerializeField] private AK.Wwise.Event stopHeartbeatEvent;

        [Header("Cue Events")] 
        [SerializeField] private VoidEventChannelSO OnHeartbeatStart;
        [SerializeField] private VoidEventChannelSO OnHeartbeatEnd;

        [Header("Cue names (MUST BE THE SAME AS IN WWISE)")] 
        [SerializeField] private string startHeartbeatCueName;
        [SerializeField] private string endHeartbeatCueName;
        
        private bool _isPlaying;
        private void OnEnable()
        {
            _isPlaying = false;
            handler?.OnPlayerBloodlust?.AddListener(HandleBloodlust);
        }

        private void OnDisable()
        {
            handler?.OnPlayerBloodlust?.RemoveListener(HandleBloodlust);
        }

        private void HandleBloodlust()
        {
            if (_isPlaying)
            {
                stopHeartbeatEvent.Post(gameObject);
                _isPlaying = false;
            }
            else
            {
                playHeartbeatEvent.Post(gameObject,
                    (uint)(AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition),
                    HandleCallbacks
                );
                _isPlaying = true;
            }
        }

        private void HandleCallbacks(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
        {
            AkMusicSyncCallbackInfo _heartbeatInfo;
            
            //check if it's music callback (beat, marker, bar, grid etc)
            if (in_info is AkMusicSyncCallbackInfo)
            {
                _heartbeatInfo = (AkMusicSyncCallbackInfo)in_info;

                if (_heartbeatInfo.musicSyncType == AkCallbackType.AK_MusicSyncUserCue)
                {
                    HandleCustomCues(_heartbeatInfo.userCueName);
                }
            }
        }

        private void HandleCustomCues(string heartbeatInfoUserCueName)
        {
            Debug.Log($"Received: {heartbeatInfoUserCueName}");
            if (heartbeatInfoUserCueName == startHeartbeatCueName)
            {
                OnHeartbeatStart?.RaiseEvent();
            }
            if(heartbeatInfoUserCueName == endHeartbeatCueName) OnHeartbeatEnd?.RaiseEvent();
        }
    }
}
