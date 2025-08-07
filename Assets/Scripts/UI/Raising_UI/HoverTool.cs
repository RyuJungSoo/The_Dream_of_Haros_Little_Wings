using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTool : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject UI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.SetActive(false);
    }
}
