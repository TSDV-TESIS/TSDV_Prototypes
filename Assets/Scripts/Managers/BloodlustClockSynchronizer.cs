using System;
using Events;
using Events.Scriptables;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Managers
{
    public class BloodlustClockSynchronizer : MonoBehaviour
    {
        [Header("Wwise events")]
        [SerializeField] private AK.Wwise.Event playHeartbeatEvent;
        [SerializeField] private AK.Wwise.Event stopHeartbeatEvent;

        
        [Header("Cue Events")] 
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;
        [SerializeField] private VoidEventChannelSO onHeartbeatStart;
        [SerializeField] private VoidEventChannelSO onHeartbeatEnd;
        [SerializeField] private AkWwiseEventChannelSO onPlayEvent;
        [SerializeField] private AkWwiseEventWithCallbackChannelSO onPlayWithCallbackEvent;
        
        [Header("Cue names (MUST BE THE SAME AS IN WWISE)")] 
        [SerializeField] private string startHeartbeatCueName;
        [SerializeField] private string endHeartbeatCueName;
        
        private bool _isPlaying;
        private void OnEnable()
        {
            _isPlaying = false;
            onBloodlustStart?.onEvent?.AddListener(HandleBloodlustStart);
            onBloodlustEnd?.onEvent?.AddListener(HandleBloodlustEnd);
        }

        private void OnDisable()
        {
            onBloodlustStart?.onEvent?.RemoveListener(HandleBloodlustStart);
            onBloodlustEnd?.onEvent?.RemoveListener(HandleBloodlustEnd);
        }

        private void HandleBloodlustEnd()
        {
            Debug.Log("Stopping?");
            if (_isPlaying)
            {
                onPlayEvent?.RaiseEvent(stopHeartbeatEvent);
                _isPlaying = false;
            }
        }

        private void HandleBloodlustStart()
        {
            if (!_isPlaying)
            {   
                onPlayWithCallbackEvent?.RaiseEvent(playHeartbeatEvent, HandleCallbacks);
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
            if (heartbeatInfoUserCueName == startHeartbeatCueName)
            {
                onHeartbeatStart?.RaiseEvent();
            }
            if(heartbeatInfoUserCueName == endHeartbeatCueName) onHeartbeatEnd?.RaiseEvent();
        }
    }
}
