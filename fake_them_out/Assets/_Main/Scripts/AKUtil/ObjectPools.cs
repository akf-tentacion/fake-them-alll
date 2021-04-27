using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

namespace AKUtil{
    public class ComponentPool<T> : ObjectPool<T>
    where T : Component
    {
        private readonly T prefab;
        private readonly Transform parentTransform;

        public ComponentPool(Transform parentTransform, T prefab)
        {
            this.parentTransform = parentTransform;
            this.prefab = prefab;
        }

        /// <summary>
        /// オブジェクトの追加生成時に実行される
        /// </summary>
        protected override T CreateInstance()
        {
            var e = GameObject.Instantiate(prefab);
            e.transform.SetParent(parentTransform);

            return e;
        }
    }

}
