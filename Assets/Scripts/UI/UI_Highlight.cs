using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Highlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite Normal_UI;
    public Sprite Highlight_UI;
    private Image image;
    [SerializeField]
    private bool isSFXUse = true;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isSFXUse)
            SoundManager.instance.PlaySFX(10, 0);
       image.sprite = Highlight_UI;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = GetComponent<Image>().sprite = Normal_UI;
    }

    public void SetNormal_UI()
    {
        image.sprite = GetComponent<Image>().sprite = Normal_UI;
    }
}
