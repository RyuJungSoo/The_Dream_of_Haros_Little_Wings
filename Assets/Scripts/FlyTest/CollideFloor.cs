using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideFloor : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    [SerializeField]
    GameOver gameover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            gameover.Gameover();
        }
    }

}
