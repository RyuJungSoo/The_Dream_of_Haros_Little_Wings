using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fail_Chance_UI : MonoBehaviour
{
    // ¿Œµ¶Ω∫ 1 : ª°∞≠
    // ¿Œµ¶Ω∫ 2 : ¡÷»≤
    // ¿Œµ¶Ω∫ 3 : ≥Î∂˚
    // ¿Œµ¶Ω∫ 4 : √ ∑œ
    // ¿Œµ¶Ω∫ 5 : ∆ƒ∂˚
    string[] Colors = { "#E42633", "#FF9E00", "#FFE341", "#9EFF7B", "#60AFFF" };

    public void UIUpdate(int Fail_Value)
    {
        TextMeshProUGUI textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = "Ω«∆–¿≤ " + Fail_Value.ToString() + "%";

        if (Fail_Value > 80)
            GetComponent<Image>().color = GetColor(0);
        else if (Fail_Value > 60)
            GetComponent<Image>().color = GetColor(1);
        else if (Fail_Value > 40)
            GetComponent<Image>().color = GetColor(2);
        else if (Fail_Value > 20)
            GetComponent<Image>().color = GetColor(3);
        else
            GetComponent<Image>().color = GetColor(4);
    }

    private Color GetColor(int index)
    {
        Color color;
        ColorUtility.TryParseHtmlString(Colors[index], out color);
        return color;
    }

}
