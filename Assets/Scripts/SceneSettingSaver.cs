using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SceneData
{
    public bool isStage1_Clear;
    public bool isStage2_Clear;
}

public class SceneSettingSaver : MonoBehaviour
{
    public static SceneSettingSaver Instance;
    public string FilePath { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            FilePath = Path.Combine(Application.persistentDataPath, "SceneData.json");
        }
    }

    private void Start()
    {
        LoadSceneData(); // 현재 스테이지 클리어 여부를 불러오기 위해 게임 실행 시 불러오기 시도
    }

    public void SaveSceneData()
    {
        var data = new SceneData
        {
            isStage1_Clear = GetComponent<SceneSettingManager>().isStage1_Clear,
            isStage2_Clear = GetComponent<SceneSettingManager>().isStage2_Clear
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? Application.persistentDataPath);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);

            Debug.Log($"저장 완료 : {FilePath}\n{json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 실패 : {e.Message}");
        }
    }

    public bool LoadSceneData()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("저장 파일 없음");
            return false;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<SceneData>(json);

            GetComponent<SceneSettingManager>().SetisStageClear(1, data.isStage1_Clear);
            GetComponent<SceneSettingManager>().SetisStageClear(2, data.isStage2_Clear);

            Debug.Log($"로드 완료 : {FilePath}\n{json}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"로드 실패 : {e.Message}");
            return false;
        }

    }

    public void ResetSave()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Debug.Log($"세이브 삭제 : {FilePath}");
            }
        }

        catch (Exception e)
        {
            Debug.LogError($"세이브 삭제 실패 : {e.Message}");
        }
    }
}
