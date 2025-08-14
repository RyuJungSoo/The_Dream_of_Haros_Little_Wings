using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideFloor : MonoBehaviour
{
    [SerializeField] Collider2D Player;
    [SerializeField] Animator playerAnim;
    [SerializeField] GameOver gameover;
    [SerializeField] Player player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            gameover.Gameover();
        }
    }
}
