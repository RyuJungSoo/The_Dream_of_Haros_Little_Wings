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
            Debug.Log("플레이어가 Wall 트리거에 접촉했습니다.");
            StartCoroutine(player.HitInvincible(20));
        }
    }

}
