using System;
using System.IO;
using UnityEngine;

[Serializable]
public class AbsoluteStats
{
    public int Stamina;
    public int FlightPower;
    public int Balance;
    public int Agility;
}

[Serializable]
public class StoreMeta
{
    public int version = 1;
    public string updatedAt; // ISO8601
    public string note;      // 선택: 어디서 저장했는지 메모
}

[Serializable]
public class StatsPayload
{
    public AbsoluteStats stats = new AbsoluteStats();
    public StoreMeta meta = new StoreMeta();
}

public static class StatsStore
{
    private const string FileName = "haroStats.v1.json";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    /// <summary>
    /// 스탯 절대값을 JSON으로 저장
    /// </summary>
    public static void Save(AbsoluteStats abs, string note = null, bool pretty = true)
    {
        if (abs == null) abs = new AbsoluteStats();

        var payload = new StatsPayload
        {
            stats = abs,
            meta = new StoreMeta
            {
                updatedAt = DateTime.UtcNow.ToString("o"),
                note = note
            }
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? Application.persistentDataPath);
            var json = JsonUtility.ToJson(payload, pretty);
            File.WriteAllText(FilePath, json);
#if UNITY_EDITOR
            Debug.Log($"[StatsStore] Saved → {FilePath}\n{json}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"[StatsStore] Save failed: {e.Message}");
        }
    }
}