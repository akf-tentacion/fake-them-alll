using UnityEditor;

namespace AKUtil
{
    public static class ToggleGameObjectActiveKeyboardShortcut
    {
        public static bool IsAvailable()
        {
            return Selection.activeGameObject != null;
        }

        // エスケープのために適当な大文字を使っているが実際はアンダーバーで起動する。
#if UNITY_EDITOR_OSX
        [MenuItem("AKUtil/Toggle GameObject ActiveSelf _A_",priority = 610)]
#else
        [MenuItem("AKUtil/Toggle GameObject ActiveSelf _¥¥",priority = 410)]
#endif
        public static void Execute()
        {
            foreach (var go in Selection.gameObjects) {
                Undo.RecordObject(go, go.name + ".activeSelf");
                go.SetActive(!go.activeSelf);
            }
        }
    }
}