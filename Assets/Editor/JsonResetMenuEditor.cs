#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class JsonResetMenuEditor
{
    [MenuItem("Tools/Save/JSON Reset (SceneData)")]
    private static void Reset_SceneData()
    {
       // JsonResetUtility.ResetSceneDataJson();
        Debug.Log("[Menu] SceneData.json 리셋 실행");
    }

    [MenuItem("Tools/Save/JSON Reset (Stats + HP + Turns)")]
    private static void Reset_All()
    {
       // JsonResetUtility.ResetStatsHpTurnsJsonAndState();
        Debug.Log("[Menu] Stats + HP + Turns 리셋 실행");
    }
}
#endif