using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWall : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    [SerializeField]
    int wallDamage = 20;

    public Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !player.isInvincible)
        {
            StartCoroutine((player.HitWall(wallDamage)));
        }
        else if(other.CompareTag("Player") && player.isInvincible)
        {
            StartCoroutine(player.HitDuringInvincible());
        }
    }

}
