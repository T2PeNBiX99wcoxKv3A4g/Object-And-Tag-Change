using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// From https://booth.pm/ja/items/6475280
namespace __yky.ObjectAndTagChange.Editor
{
    public class SetObjectAndTag : EditorWindow
    {
        private static readonly Dictionary<int, string> OriginalTags = new();
        private static readonly Dictionary<int, bool> WasActives = new();
        private const string EditorOnlyTag = "EditorOnly";

        [MenuItem("Tools/Set Object and Tag #e")]
        [MenuItem("GameObject/yky/Set Object and Tag")]
        private static void ToggleInactiveAndTag()
        {
            var selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length < 1)
            {
                Debug.LogWarning("オブジェクトが選択されていません。");
                return;
            }

            foreach (var obj in selectedObjects)
            {
                var id = obj.GetInstanceID();
                
                if (obj.activeSelf)
                {
                    if (!OriginalTags.ContainsKey(id))
                    {
                        OriginalTags.TryAdd(id, obj.tag);
                        WasActives.TryAdd(id, obj.activeSelf);
                    }
                    
                    Undo.RecordObject(obj, "Change EditorOnly");
                    obj.SetActive(false);
                    obj.tag = EditorOnlyTag;
                }
                else
                {
                    var hasOriginalTag = OriginalTags.TryGetValue(id, out var originalTag);
                    var hasActive = WasActives.TryGetValue(id, out var wasActive);

                    Undo.RecordObject(obj, "Undo Tag");
                    obj.SetActive(!hasActive || wasActive);
                    if (obj.CompareTag(EditorOnlyTag))
                        obj.tag = hasOriginalTag ? originalTag : "";

                    OriginalTags.Remove(id);
                    WasActives.Remove(id);
                }

                EditorUtility.SetDirty(obj);
            }
        }
    }
}