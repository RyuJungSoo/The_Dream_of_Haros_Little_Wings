
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


    [Serializable]
    public class SaveGameData
    {
        public int currentTurn;
        public int maxTurn;

        public int staminaStat;
        public int flightpowerStat;
        public int balanceStat;
        public int agilityStat;

        public float currentStamina;
        public float maxStamina;
    }


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public string FilePath { get; private set; }
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        FilePath = Path.Combine(Application.persistentDataPath, "haroSave.v1.json");
        Debug.Log($"[SaveManager] Save Path = {FilePath}");
    }


    public void SaveGame()
    {

        if (File.Exists(FilePath))
        {

            var data = new SaveGameData
            {
                currentTurn = GameManager.Instance.CurrentTurn,
                maxTurn = GameManager.Instance.maxTurn,
                staminaStat = StatManager.Instance.Stamina_Stat,
                flightpowerStat = StatManager.Instance.Flightpower_Stat,
                balanceStat = StatManager.Instance.Balance_Stat,
                agilityStat = StatManager.Instance.Agility_Stat,
                currentStamina = StatManager.Instance.currentStamina,
                maxStamina = StatManager.Instance.maxStamina,

            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
        else
        {
            Debug.LogWarning("[SaveManager] 저장 파일 없음. 메모리 기본값 유지");
        }

    }

    public void LoadGame()
    {
        
        
            Debug.Log($"[SaveManager] 로드 시도: {FilePath}, Exists={File.Exists(FilePath)}");

            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                var data = JsonUtility.FromJson<SaveGameData>(json);

                GameManager.Instance.CurrentTurn = data.currentTurn;
                GameManager.Instance.maxTurn = data.maxTurn;
                StatManager.Instance.Stamina_Stat = data.staminaStat;
                StatManager.Instance.Flightpower_Stat = data.flightpowerStat;
                StatManager.Instance.Balance_Stat = data.balanceStat;
                StatManager.Instance.Agility_Stat = data.agilityStat;
                StatManager.Instance.currentStamina = data.currentStamina;
                StatManager.Instance.maxStamina = data.maxStamina;

            }
            else
            {

                InitGame();

            }
        


    }
    public void InitGame()
    {
        var data = new SaveGameData
        {
            currentTurn = 15,
            maxTurn = 15,
            staminaStat = 0,
            flightpowerStat = 0,
            balanceStat = 0,
            agilityStat = 0,
            currentStamina = 100f,
            maxStamina = 100f,

        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }
    //리셋관련 
    public void ResetGame()
    {
        SaveManager.Instance.InitGame();
        SceneSettingSaver.Instance.ResetSave();
        SceneSettingSaver.Instance.LoadSceneData();
        UnityEngine.Debug.Log("저장데이터 리셋");
    }
    
    public void ResetGameTurn()
    {
        try
        {
            Debug.Log($"[SaveManager] 로드 시도: {FilePath}, Exists={File.Exists(FilePath)}");

            if (File.Exists(FilePath))
            {
                string savedjson = File.ReadAllText(FilePath);
                var savedata = JsonUtility.FromJson<SaveGameData>(savedjson);

                var data = new SaveGameData
                {
                    currentTurn = savedata.maxTurn,
                    maxTurn = savedata.maxTurn,
                    staminaStat = savedata.staminaStat,
                    flightpowerStat = savedata.flightpowerStat,
                    balanceStat = savedata.balanceStat,
                    agilityStat = savedata.agilityStat,
                    currentStamina = savedata.currentStamina,
                    maxStamina = savedata.maxStamina,

                };

                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(FilePath, json);


            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 로드 실패: {e.Message}");
        }

    }
}

