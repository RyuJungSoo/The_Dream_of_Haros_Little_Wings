using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txt;

    [SerializeField]
    Player player;

    void Start()
    {
        txt.enabled = false;
    }

    public void Gameclear()
    {
        txt.enabled = true;
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
