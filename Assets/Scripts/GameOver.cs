using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        txt.enabled = false;
    }

    public void Gameover()
    {
        txt.enabled = true;
    }
}
