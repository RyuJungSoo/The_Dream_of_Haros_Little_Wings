using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 3f;

    [SerializeField]
    GameOver gameover;

    [SerializeField]
    Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        if (!gameover.gameover)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
    }
}
