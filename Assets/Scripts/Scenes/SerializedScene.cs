using System;
using UnityEngine.Events;

namespace Scenes.ScriptableObjects
{
    [Serializable]
    public class SerializedScene
    {
        public string sceneName;
        public string sceneGuid;
        public int index;

        public UnityEvent onLoad = new UnityEvent();
        public UnityEvent onUnload = new UnityEvent();
    }
}