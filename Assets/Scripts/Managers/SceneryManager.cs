using System;
using System.Collections.Generic;
using Events;
using Events.ScriptableObjects;
using Events.Scriptables;
using Scenes.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneryManager : MonoBehaviour
    {
        [SerializeField] private ScenesDataConfig scenesDataConfig;
        
        [Tooltip("Scenes that are on boot")] 
        [SerializeField] private string[] initScenes;

        [Header("events")] 
        [SerializeField] private StringEventChannelSO onLoadScene;
        [SerializeField] private StringEventChannelSO onUnloadScene;
        [SerializeField] private StringEventChannelSO onSetActiveScene;
        [SerializeField] private SubscribeToSceneChannelSO onSubscribeToScene;
        [SerializeField] private SubscribeToSceneChannelSO onUnsubscribeToScene;
        
        private readonly List<SerializedScene> _activeScenes = new List<SerializedScene>();

        private void OnEnable()
        {
            onLoadScene?.onTypedEvent.AddListener(LoadScene);
            onUnloadScene?.onTypedEvent.AddListener(UnloadScene);
            onSetActiveScene?.onTypedEvent.AddListener(SetActiveScene);
            onSubscribeToScene?.onTypedEvent.AddListener(SubscribeEventToAddScene);
            onUnsubscribeToScene?.onTypedEvent.AddListener(UnsubscribeEventToAddScene);
        }

        private void OnDisable()
        {
            onLoadScene?.onTypedEvent.RemoveListener(LoadScene);
            onUnloadScene?.onTypedEvent.RemoveListener(UnloadScene);
            onSetActiveScene?.onTypedEvent.RemoveListener(SetActiveScene);
            onSubscribeToScene?.onTypedEvent.RemoveListener(SubscribeEventToAddScene);
            onUnsubscribeToScene?.onTypedEvent.RemoveListener(UnsubscribeEventToAddScene);
        }

        /// <summary>
        /// Loads all init scenes.
        /// </summary>
        public void InitScenes()
        {
            foreach (var initScene in initScenes)
            {
                LoadScene(initScene);
            }
        }

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="sceneName">The scene name to load.</param>
        public void LoadScene(string sceneName)
        {
            if (sceneName == "Exit")
            {
                Debug.Log("Quitting...");
                Application.Quit();
                return;
            }

            SerializedScene scene = scenesDataConfig.GetSerializedScene(sceneName);
            
            scene.onLoad?.Invoke();
            AddScene(scene);
        }

        /// <summary>
        /// Unloads the scene.
        /// </summary>
        /// <param name="aSceneName">The scene name to unload.</param>
        public void UnloadScene(string aSceneName)
        {
            SerializedScene aScene = scenesDataConfig.GetSerializedScene(aSceneName);
            
            if (_activeScenes.Exists(scene => scene.sceneName == aScene.sceneName))
            {
                SceneManager.UnloadSceneAsync(aScene.index);
                aScene.onUnload?.Invoke();
                _activeScenes.RemoveAt(_activeScenes.FindIndex(scene => scene.sceneName == aScene.sceneName));
            }
            else
            {
                Debug.LogWarning($"{aScene.sceneName} not active!");
            }
        }

        /// <summary>
        /// Loads a new scene with scene mode aditive.
        /// </summary>
        /// <param name="scene">Serializable scene to load.</param>
        private void AddScene(SerializedScene scene)
        {
            SceneManager.LoadScene(scene.index, LoadSceneMode.Additive);
            _activeScenes.Add(scene);
        }

        /// <summary>
        /// Subscribes an event to an Add scene event
        /// </summary>
        public void SubscribeEventToAddScene(SubscribeToSceneData eventData)
        {
            SerializedScene aScene = scenesDataConfig.GetSerializedScene(eventData.sceneName);
            
            aScene.onLoad.AddListener(eventData.SubscribeToSceneAction);
        }
        
        /// <summary>
        /// Unsubscribes an event from the Add scene event
        /// </summary>
        public void UnsubscribeEventToAddScene(SubscribeToSceneData eventData)
        {
            SerializedScene aScene = scenesDataConfig.GetSerializedScene(eventData.sceneName);

            aScene.onLoad.RemoveListener(eventData.SubscribeToSceneAction);
        }

        public void SetActiveScene(string sceneName)
        {
            SerializedScene aScene = scenesDataConfig.GetSerializedScene(sceneName);
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(aScene.sceneName));
        }
    }
}
