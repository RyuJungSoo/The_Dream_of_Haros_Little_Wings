using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SceneData
{
    // 스테이지 클리어 여부 플래그
    public bool isStage1_Clear;
    public bool isStage2_Clear;
}

public class SceneSettingSaver : MonoBehaviour
{
    public static SceneSettingSaver Instance;

    // 저장/로드할 JSON 파일 경로
    public string FilePath { get; private set; }

    private void Awake()
    {
        // 싱글턴 설정 및 파일 경로 초기화
        if (Instance == null)
        {
            Instance = this;
            FilePath = Path.Combine(Application.persistentDataPath, "SceneData.json");
        }
    }

    private void Start()
    {
        // 게임 시작 시 저장된 씬 데이터 로드 시도
        LoadSceneData();
    }

    /// <summary>
    /// 현재 씬의 클리어 상태를 JSON으로 저장
    /// </summary>
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

            Debug.Log($"[SceneSettingSaver] 저장 완료 : {FilePath}\n{json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SceneSettingSaver] 저장 실패 : {e.Message}");
        }
    }

    /// <summary>
    /// 저장된 씬 클리어 상태를 로드하여 적용
    /// </summary>
    public bool LoadSceneData()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("[SceneSettingSaver] 저장 파일이 없습니다.");
            return false;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<SceneData>(json);

            GetComponent<SceneSettingManager>().SetisStageClear(1, data.isStage1_Clear);
            GetComponent<SceneSettingManager>().SetisStageClear(2, data.isStage2_Clear);

            Debug.Log($"[SceneSettingSaver] 로드 완료 : {FilePath}\n{json}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SceneSettingSaver] 로드 실패 : {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 클리어 상태를 초기값으로 재설정(파일을 기본값으로 덮어씀).
    /// </summary>
    public void ResetSave()
    {
        try
        {
            var data = new SceneData
            {
                isStage1_Clear = false,
                isStage2_Clear = false
            };

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? Application.persistentDataPath);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);

            Debug.Log($"[SceneSettingSaver] 초기화 완료 : {FilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SceneSettingSaver] 초기화 실패 : {e.Message}");
        }
    }
}
