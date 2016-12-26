using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _Instance = null;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<T>();

                if (_Instance == null)
                    Debug.Log("Instance of " + typeof(T).ToString() + " is Null");

                return _Instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }
}