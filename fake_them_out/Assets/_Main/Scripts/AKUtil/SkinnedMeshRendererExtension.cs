using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AKUtil
{
    /// <summary>
    /// SkinnedMeshRendererのボーン構造をコピーする便利クラスです。
    /// 既に存在するキャラクターのPrefabに、ボーン構造が同じ新しいスキンを追加したいときなどに使います。
    /// </summary>
    public static class SkinnedMeshRendererExtension
    {
        public static void CopyBones(Transform root, SkinnedMeshRenderer before, SkinnedMeshRenderer newMeshRenderer)
        {
            // update mesh
            var meshrenderer = before;
            meshrenderer.sharedMesh = newMeshRenderer.sharedMesh;
            Transform[] childrens = root.GetComponentsInChildren<Transform>(true);
            // sort bones.
            Transform[] bones = new Transform[newMeshRenderer.bones.Length];
            for (int boneOrder = 0; boneOrder < newMeshRenderer.bones.Length; boneOrder++)
            {
                bones[boneOrder] = Array.Find<Transform>(childrens, c => c.name == newMeshRenderer.bones[boneOrder].name);
            }
            meshrenderer.bones = bones;
        }
    }

}
