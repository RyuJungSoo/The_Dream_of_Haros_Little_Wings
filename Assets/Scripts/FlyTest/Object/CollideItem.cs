using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideItem : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어가 Item을 획득했습니다.");
        }
    }

}
