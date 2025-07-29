using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    public Image barFiller;

    public void UpdateBar(float current, float max)
    {
        if (barFiller == null) return;

        float fillAmount = current / max;
        barFiller.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
