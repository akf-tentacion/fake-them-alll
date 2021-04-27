using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKUtil
{
    /// <summary>
    /// singletonパターンで実装されるmonobehaviour
    /// 参照時にインスタンスが存在しない場合、gameobjectにadd componentされて自動生成される。
    /// デフォルトで、DontDestroyOnLoad しません。
    /// </summary>

    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        protected static T instance;
        public static T Instance {get { return CreateInstance(); } }

        public static T CreateInstance()
        {
            if (instance != null) return instance;

            instance = (T)FindObjectOfType(typeof(T));

            if (instance != null) return instance;

            var name = typeof(T).Name;
            Debug.LogFormat("Create singleton object: {0}", name);
            GameObject obj = new GameObject(name);
            instance = obj.AddComponent<T>();

            if (instance != null) return instance;

            Debug.LogWarning("Can't find singleton object: " + typeof(T).Name);
            Debug.LogError("Can't create singleton object: " + typeof(T).Name);

            return null;
        }
        public static bool IsInstantiated()
        {
            return (instance != (MonoBehaviour)null);
        }

        private void Awake()
        {
            if (CheckInstance())AwakeValidly();
        }

        protected virtual void AwakeValidly()
        {
            // do nothing
        }

        private bool CheckInstance()
        {
            if (instance == null)
            {
                instance = (T)this;
                return true;
            }

            if (Instance == this) return true;

            Destroy(this);
            return false;
        }

        protected void DontDestroyOnLoad()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }
}
