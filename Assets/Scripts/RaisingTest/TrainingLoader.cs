using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TrainingLoader : MonoBehaviour
{
    public GameObject loadingPanel;
    public Image progressBarFiller;
    public float loadingTime = 3f;

    public GameObject loadingText;
    public GameObject successText;
    public GameObject failText;

    private float timer = 0f;
    private bool isLoading = false;

    private StatType currentStat;

    float baseSpeed = 1f;       // 기본 속도 배율
    float boostSpeed = 3f;      // 클릭 시 속도 배율

    public void StartTraining(StatType statType)
    {
        Debug.Log($"[TrainingLoader] StartTraining: {statType}");

        currentStat = statType;

        // 초기화
        loadingPanel.SetActive(true);
        progressBarFiller.fillAmount = 0f;
        loadingText.SetActive(true);
        successText.SetActive(false);
        failText.SetActive(false);

        timer = 0f;
        isLoading = true;
    }

    void Update()
    {
        if (!isLoading) return;

        float speedMultiplier = baseSpeed;

        if (Input.GetMouseButton(0))
        {
            speedMultiplier = boostSpeed;
        }

        timer += Time.deltaTime * speedMultiplier;
        progressBarFiller.fillAmount = timer / loadingTime;

        if (timer >= loadingTime)
        {
            CompleteTraining();
        }
    }

    void CompleteTraining()
    {
        isLoading = false;
        StartCoroutine(ShowResult()); // 코루틴 실행
    }

    IEnumerator ShowResult()
    {
        loadingText.SetActive(false);

        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float failureRate = 1f - (currentStamina / maxStamina);
        float rand = Random.value;

        Debug.Log($"[Training] 실패율: {failureRate:P1}, 랜덤값: {rand:F2}");

        bool isSuccess = rand >= failureRate;

        if (isSuccess)
        {
            successText.SetActive(true);
            StatManager.Instance.IncreaseStat(currentStat);
        }
        else
        {
            failText.SetActive(true);
        }

        UIManager.Instance.UpdateStatUI();
        GameManager.Instance.UseTurn();

        yield return new WaitForSeconds(1.5f);
        loadingPanel.SetActive(false);
    }
}
