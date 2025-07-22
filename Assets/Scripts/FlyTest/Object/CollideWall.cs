using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWall : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    public Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !player.isInvincible)
        {
            StartCoroutine(player.HitInvincible(20));
        }
        else if(other.CompareTag("Player") && player.isInvincible)
        {
            StartCoroutine(player.HitDuringInvincible());
        }
    }

}
