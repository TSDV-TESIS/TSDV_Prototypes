using System;
using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Events.Scriptables
{
    [Serializable]
    public class SubscribeToSceneData
    {
        public string sceneName;
        public UnityAction SubscribeToSceneAction;
    }
    
    [CreateAssetMenu(menuName = "Events/Subscribe To Scene Channel")]
    public class SubscribeToSceneChannelSO : EventChannelSO<SubscribeToSceneData>
    {
    
    }
}
