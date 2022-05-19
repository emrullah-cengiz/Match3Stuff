using Assets._Game.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public abstract class Singleton<T> : MonoBehaviour, ICustomMBActions where T : MonoBehaviour
    {
        public static T Instance
        {
            get;
            private set;
        }

        public virtual void _Awake() { }
        public virtual void _OnDestroy() { }

        private void Awake()
        {
            if (Instance == null || Instance != this)
                Instance = this as T;

            _Awake();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            _OnDestroy();
        }


    }
}