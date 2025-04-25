using System;
using Events.Scriptables;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event stopAllSoundEvent;

        [Header("Events")] 
        [SerializeField] private AkWwiseEventChannelSO onPlayEvent;
        [SerializeField] private AkWwiseEventChannelSO onStopEvent;
        [SerializeField] private AkWwiseEventWithCallbackChannelSO onPlayEventWithCallback;

        public void OnEnable()
        {
            onPlayEvent?.onTypedEvent.AddListener(PlayEvent);
            onPlayEventWithCallback?.onTypedEvent.AddListener(PlayEvent);
            onStopEvent?.onTypedEvent.AddListener(StopEvent);
        }

        public void OnDisable()
        {
            onPlayEvent?.onTypedEvent.RemoveListener(PlayEvent);
            onPlayEventWithCallback?.onTypedEvent.RemoveListener(PlayEvent);
            onStopEvent?.onTypedEvent.RemoveListener(StopEvent);
            stopAllSoundEvent?.Post(gameObject);
        }

        private void StopEvent(AK.Wwise.Event anEvent)
        {
            anEvent.Stop(gameObject);
        }
        
        private void PlayEvent(AK.Wwise.Event anEvent)
        {
            anEvent.Post(gameObject);
        }

        private void PlayEvent(AK.Wwise.Event anEvent, AkCallbackManager.EventCallback callback)
        {
            anEvent.Post(gameObject,
                (uint)(AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition), callback);
        }
    }
}
