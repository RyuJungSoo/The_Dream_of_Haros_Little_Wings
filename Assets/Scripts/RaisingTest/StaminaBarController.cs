using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    [SerializeField] private Image barFiller;
    public Slider slider;
    [Range(0f, 1f)] public float firstDropMaxDelta = 0.03f; // 최초 하락 최대 3%
    [Range(0f, 1f)] public float firstDropScale = 0.25f;    // 최초 하락량을 25%만 표시

    private float _lastFill = -1f;
    private bool _firstDropApplied = false;

    public void UpdateBar(float current, float max)
    {
        //barFiller.fillAmount = current;
        if (!barFiller || max <= 0f) return;

        float target = Mathf.Clamp01(current / max);

        if (_lastFill < 0f)
            _lastFill = barFiller.fillAmount > 0f ? Mathf.Clamp01(barFiller.fillAmount) : 1f;

        float delta = target - _lastFill;

        if (!_firstDropApplied && delta < 0f)
        {
            float scaled = delta * Mathf.Clamp01(firstDropScale);
            float capped = Mathf.Max(scaled, -firstDropMaxDelta);
            float newFill = Mathf.Clamp01(_lastFill + capped);
            barFiller.fillAmount = newFill;
            _lastFill = newFill;
            _firstDropApplied = true;
        }
        else
        {
            barFiller.fillAmount = target;
            _lastFill = target;
        }
    }

    public void ResetFirstDrop()
    {
        _firstDropApplied = false;
        _lastFill = -1f;
    }
}
