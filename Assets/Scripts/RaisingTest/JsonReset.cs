/*#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class StatManagerEditorMenu
{
    [MenuItem("Tools/Save/Reset stat_data.json")]
    private static void EditorResetSave()
    {
        var path = Path.Combine(Application.persistentDataPath, "stat_data.json");
        if (File.Exists(path)) File.Delete(path);
        Debug.Log($"[StatManager] Reset: {path}");
    }

    [MenuItem("Tools/Save/Open PersistentData Folder")]
    private static void EditorOpenPersistentFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
#endif
*/