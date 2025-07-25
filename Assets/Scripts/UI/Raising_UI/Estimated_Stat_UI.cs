using UnityEngine;
using TMPro;

public class StatIncreaseUI : MonoBehaviour
{
    private TextMeshProUGUI textUI;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    public void Show(string valueText, bool isMainStat)
    {
        if (textUI == null) return;

        textUI.text = valueText;
        textUI.fontSize = 50;
        textUI.color = Color.white; // 내부에 color 태그 있으므로 white로 유지

        textUI.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (textUI != null)
            textUI.gameObject.SetActive(false);
    }
}
