using System;
using System.Collections;
using Events;
using UnityEngine;
using Events.Scriptables;

namespace Scenes
{
    public class SceneHandler : MonoBehaviour
    {
        [SerializeField] private string[] scenesToSubscribeTo;
        [Tooltip("Current scene name")] [SerializeField] private string sceneName;
        [Tooltip("Optional scenes to activate with the current scene")] [SerializeField] private string[] optionalScenes = new string[] {};
        [SerializeField] private bool setAsActiveOnBoot = false;
        [SerializeField] private AK.Wwise.State musicEvent;
        
        [Header("events")] 
        [SerializeField] private StringEventChannelSO onLoadSceneEvent;
        [SerializeField] private StringEventChannelSO onUnloadSceneEvent;
        [SerializeField] private StringEventChannelSO onSetActiveSceneEvent;
        [SerializeField] private SubscribeToSceneChannelSO onSubscribeToSceneEvent;
        [SerializeField] private SubscribeToSceneChannelSO onUnSubscribeToSceneEvent;

        [SerializeField] private VoidEventChannelSO onSceneUnloadedEvent;
        [SerializeField] private BoolEventChannelSO[] onHandleCanvasesEvents;
        private void Start()
        {
            foreach (var optionalScene in optionalScenes)
            {
                onLoadSceneEvent?.RaiseEvent(optionalScene);
            }

            if (setAsActiveOnBoot)
            {
                StartCoroutine(SetSceneAsActiveScene());
            }

            if (musicEvent.GroupId != 0)
            {
                Debug.Log(musicEvent.GroupId);
                Debug.Log(musicEvent.Id);
                AkSoundEngine.SetState(musicEvent.GroupId, musicEvent.Id);
            }
            
            SubscribeToActions();
            HandleCanvasEvents(true);
        }

        private void OnDisable()
        {
            UnsubscribeToActions();
            HandleCanvasEvents(false);
        }

        private IEnumerator SetSceneAsActiveScene()
        {
            // returns a yield null as it needs a frame to load the scene, then it can be set
            // as active.
            yield return null;
            onSetActiveSceneEvent?.RaiseEvent(sceneName);
        }
        
        /// <summary>
        /// Subscribes to add scene event of the scenes to subscribe to.
        /// </summary>
        private void SubscribeToActions()
        {
            Array.ForEach(scenesToSubscribeTo, (aSceneName) =>
            {
                onSubscribeToSceneEvent?.RaiseEvent(new SubscribeToSceneData()
                {
                    sceneName = aSceneName,
                    SubscribeToSceneAction = UnloadScene
                });
            });
        }
        
        /// <summary>
        /// Unsubscribes to add scene event of the scenes to subscribe to.
        /// </summary>
        private void UnsubscribeToActions()
        {
            Array.ForEach(scenesToSubscribeTo, (aSceneName) =>
            {
                onUnSubscribeToSceneEvent?.RaiseEvent(new SubscribeToSceneData()
                {
                    sceneName = aSceneName,
                    SubscribeToSceneAction = UnloadScene
                });
            });
        }
        
        /// <summary>
        /// Unloads current scene and optional scenes.
        /// </summary>
        private void UnloadScene()
        {
            onUnloadSceneEvent?.RaiseEvent(sceneName);
            onSceneUnloadedEvent?.RaiseEvent();
            
            foreach (var optionalScene in optionalScenes)
            {
                onUnloadSceneEvent?.RaiseEvent(optionalScene);
            }
        }

        /// <summary>
        ///  Activates or deactivates canvas on scene init
        /// </summary>
        private void HandleCanvasEvents(bool value)
        {
            foreach (var onHandleCanvasEvent in onHandleCanvasesEvents)
            {
                onHandleCanvasEvent?.RaiseEvent(value);
            }
        }
    }
}
