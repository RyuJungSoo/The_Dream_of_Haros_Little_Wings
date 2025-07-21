using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txt;

    [SerializeField]
    Player player;

    [SerializeField]
    public bool gameover = false;

    // Start is called before the first frame update
    void Start()
    {
        txt.enabled = false;
    }

    public void Gameover()
    {
        txt.enabled = true;
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        gameover = true;
    }
}
