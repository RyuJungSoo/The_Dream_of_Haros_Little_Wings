using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWall : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ Wall�� �浹�߽��ϴ�.");
        }
    }

}
