using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prologue : MonoBehaviour
{
    [SerializeField]
    private GameObject Background_Object;
    [SerializeField]
    private GameObject Image_Object;
    [SerializeField]
    private GameObject Text_Object;
    [SerializeField]
    private GameObject SkipButton_Object;
    [SerializeField]
    private Button StartButton_Object;
    [SerializeField]
    private Button QuitButton_Object;
    [SerializeField]
    private float duration = 1f; // fade 지속 시간 (초)

    public void SequenceStart()
    {
        StartButton_Object.interactable = false;
        QuitButton_Object.interactable = false;
        Background_Object.SetActive(true);
        Image_Object.SetActive(true);

        StartCoroutine("SetBackground");
    }

    IEnumerator SetBackground()
    {
        float timer = 0f;
        Color color = Background_Object.GetComponent<Image>().color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            Background_Object.GetComponent<Image>().color = color;
            yield return null;
        }
        StartCoroutine("SetImage");
    }

    IEnumerator SetImage()
    {
        float timer = 0f;
        Color color = Image_Object.GetComponent<Image>().color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            Image_Object.GetComponent<Image>().color = color;
            yield return null;
        }

        Text_Object.SetActive(true);
        GetComponent<Dialogue_Setting>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        
        GetComponent<Dialogue_Setting>().DialogueSetting();
        SkipButton_Object.SetActive(true);
    }
}
